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
        var codeSigner = new CodeSigner(new AmazonSignerClient(), new AmazonS3Client());

        var response = await codeSigner.Handle(input, context);

        return response;
    }
}
