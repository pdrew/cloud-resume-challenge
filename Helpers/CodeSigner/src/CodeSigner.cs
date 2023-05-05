using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Signer;
using Amazon.Signer.Model;

namespace CodeSigner;

public class CodeSigner
{
    private readonly IAmazonSigner signerClient;
    private readonly IAmazonS3 s3Client;
    
    public CodeSigner(IAmazonSigner signerClient, IAmazonS3 s3Client)
    {
        this.signerClient = signerClient;
        this.s3Client = s3Client;
    }
    
    public async Task<CustomResourceResponse> Handle(CustomResourceEvent<SignLambdaRequest> input, ILambdaContext context)
    {
        context.Logger.Log(JsonSerializer.Serialize(input));

        switch (input.RequestType)
        {
            case "Create":
            case "Update":
                var signedObject = await SignLambda(input.ResourceProperties);
                return new CustomResourceResponse()
                {
                    Data = new Dictionary<string, object>()
                    {
                        { "BucketName", signedObject.S3.BucketName },
                        { "Key", signedObject.S3.Key }   
                    }
                };
            case "Delete":
                context.Logger.Log("Received delete request");
                break;
        }

        return await Task.FromResult(new CustomResourceResponse());
    }

    private async Task<SignedObject> SignLambda(SignLambdaRequest signLambdaRequest)
    {
        var objectVersion = await GetObjectVersion(signLambdaRequest.BucketName, signLambdaRequest.ObjectKey);
        
        var request = new StartSigningJobRequest()
        {
            ProfileName = signLambdaRequest.ProfileName,
            Source = new Source()
            {
                S3 = new S3Source()
                {
                    BucketName = signLambdaRequest.BucketName,
                    Key = signLambdaRequest.ObjectKey,
                    Version = objectVersion.VersionId
                }
            },
            Destination = new Destination()
            {
                S3 = new S3Destination()
                {
                    BucketName = signLambdaRequest.BucketName,
                    Prefix = "Signed/"
                }
            }
        };

        var response = await signerClient.StartSigningJobAsync(request);

        var signedObject = await WaitForSigningJobResult(response.JobId);

        return signedObject;
    }
    
    private async Task<SignedObject> WaitForSigningJobResult(string jobId)
    {
        while (true)
        {
            var request = new DescribeSigningJobRequest()
            {
                JobId = jobId
            };

            var response = await signerClient.DescribeSigningJobAsync(request);

            if (response.Status == SigningStatus.Succeeded)
            {
                return response.SignedObject;
            }

            if (response.Status == SigningStatus.Failed)
            {
                Console.WriteLine(JsonSerializer.Serialize(response));

                throw new ApplicationException($"Signing Job failed: {response.StatusReason}");
            }

            await Task.Delay(500);
        }
    }
    
    private async Task<S3ObjectVersion> GetObjectVersion(string bucketName, string objectKey)
    {
        var request = new ListVersionsRequest()
        {
            BucketName = bucketName,
            Prefix = objectKey
        };
        
        var response = await s3Client.ListVersionsAsync(request);

        var version = response.Versions.FirstOrDefault(x => x.IsLatest)
                      ?? throw new ApplicationException(
                          $"No version found for object. Bucket: {bucketName} Object: {objectKey}");
        
        return version;
    }
}