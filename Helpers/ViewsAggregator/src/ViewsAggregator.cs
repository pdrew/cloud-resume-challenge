using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Util;
using BackEnd.Shared.Models;
using BackEnd.Shared.Services;

namespace ViewsAggregator;

public class ViewsAggregator
{
    private readonly IDynamoDBContext db;

    private readonly IDateTimeProvider dateTimeProvider;
    
    public ViewsAggregator(IDynamoDBContext db, IDateTimeProvider dateTimeProvider)
    {
        this.db = db;
        this.dateTimeProvider = dateTimeProvider;
        
        AWSConfigsDynamoDB.Context.AddMapping(new TypeMapping(
            typeof(ViewStatistics), Environment.GetEnvironmentVariable("DYNAMODB_TABLE")));

        AWSConfigsDynamoDB.Context.AddMapping(new TypeMapping(
            typeof(Visitor), Environment.GetEnvironmentVariable("DYNAMODB_TABLE")));
    }

    public async Task Handle(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

        var month = dateTimeProvider.GetCurrentYearAndMonthDatePartString();
        
        var statistics = await db.LoadAsync<ViewStatistics>("STATISTICS", month) ?? new ViewStatistics(month);

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

            var delta = newImage.TotalViews - oldImage.TotalViews;

            statistics.TotalViews += delta;
        }

        await db.SaveAsync(statistics);

        context.Logger.LogInformation("Stream processing complete.");
    }
    
    private T GetObject<T>(Dictionary<string, AttributeValue> image)
    {
        var document = Document.FromAttributeMap(image);
        return db.FromDocument<T>(document);
    }
}