using System.Text.Json;
using Amazon.Lambda.Core;

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
    public CustomResourceResponse FunctionHandler(CustomResourceEvent input, ILambdaContext context)
    {
        context.Logger.Log(JsonSerializer.Serialize(input));

        switch (input.RequestType)
        {
            case "Create":
                context.Logger.Log("Creating stuff");
                break;
            case "Update":
                context.Logger.Log("Updating stuff");
                break;
            case "Delete":
                context.Logger.Log("Deleting stuff");
                break;
        }

        return new CustomResourceResponse()
        {
            PhysicalResourceId = input.PhysicalResourceId,
            
            Data = new Dictionary<string, object>()
            {
                { "Foo", "Bar" }
            }
        };
    }
}
