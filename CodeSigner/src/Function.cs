using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Signer;
using Amazon.Signer.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CodeSigner;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<CustomResourceResponse> FunctionHandler(CustomResourceEvent<SignLambdaRequest> input, ILambdaContext context)
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
                context.Logger.Log("Deleting stuff");
                break;
        }

        return await Task.FromResult(new CustomResourceResponse());
    }

    private async Task<SignedObject> SignLambda(SignLambdaRequest signLambdaRequest)
    {
        var objectVersion = await GetObjectVersion(signLambdaRequest.BucketName, signLambdaRequest.ObjectKey);
        
        var client = new AmazonSignerClient();

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

        var response = await client.StartSigningJobAsync(request);

        var signedObject = await WaitForSigningJobResult(response.JobId);

        return signedObject;
    }

    private async Task<SignedObject> WaitForSigningJobResult(string jobId)
    {
        var client = new AmazonSignerClient();

        while (true)
        {
            var request = new DescribeSigningJobRequest()
            {
                JobId = jobId
            };

            var response = await client.DescribeSigningJobAsync(request);

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
        var client = new AmazonS3Client();

        var request = new ListVersionsRequest()
        {
            BucketName = bucketName,
            Prefix = objectKey
        };
        
        var response = await client.ListVersionsAsync(request);

        var version = response.Versions.FirstOrDefault(x => x.IsLatest)
                      ?? throw new ApplicationException(
                          $"No version found for object. Bucket: {bucketName} Object: {objectKey}");
        
        return version;
    }
}
