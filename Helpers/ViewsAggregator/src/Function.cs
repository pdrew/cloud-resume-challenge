using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using ViewsAggregator.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ViewsAggregator;

public class Function
{
    private readonly DynamoDBContext _context;

    public Function()
    {
        _context = new DynamoDBContext(new AmazonDynamoDBClient());
    }
    public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

        foreach (var record in dynamoEvent.Records)
        {
            var visitor = GetObject<Visitor>(record.Dynamodb.OldImage);
            context.Logger.LogInformation($"Old Image {JsonSerializer.Serialize(visitor)}");
            
            visitor = GetObject<Visitor>(record.Dynamodb.NewImage);
            context.Logger.LogInformation($"New Image {JsonSerializer.Serialize(visitor)}");
            
            // TODO: Add business logic processing the record.Dynamodb object.
        }

        context.Logger.LogInformation("Stream processing complete.");
    }
    
    private T GetObject<T>(Dictionary<string, AttributeValue> image)
    {
        var document = Document.FromAttributeMap(image);
        return _context.FromDocument<T>(document);
    }
}