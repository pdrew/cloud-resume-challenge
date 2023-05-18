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
    private readonly DynamoDBContext db;

    public Function()
    {
        db = new DynamoDBContext(new AmazonDynamoDBClient());
    }
    public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");
        
        var statistics = await db.LoadAsync<ViewStatistics>("STATISTICS", "VIEWS") ?? new ViewStatistics();

        foreach (var record in dynamoEvent.Records)
        {
            var oldImage = GetObject<Visitor>(record.Dynamodb.OldImage);
            context.Logger.LogInformation($"Old Image {JsonSerializer.Serialize(oldImage)}");
            
            var newImage = GetObject<Visitor>(record.Dynamodb.NewImage);
            context.Logger.LogInformation($"New Image {JsonSerializer.Serialize(newImage)}");

            if (string.Equals(record.EventName,"INSERT", StringComparison.CurrentCultureIgnoreCase))
            {
                statistics.UniqueVisitors++;
            }

            var delta = newImage.Views - oldImage.Views;

            statistics.TotalViews += delta;
        }

        context.Logger.LogInformation("Stream processing complete.");
    }
    
    private T GetObject<T>(Dictionary<string, AttributeValue> image)
    {
        var document = Document.FromAttributeMap(image);
        return db.FromDocument<T>(document);
    }
}