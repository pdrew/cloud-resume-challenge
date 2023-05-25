using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using Amazon.Util;
using BackEnd.Shared.Models;
using BackEnd.Shared.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ViewsAggregator;

public class Function
{
    private ViewsAggregator viewAggregator;

    public Function()
    {
        viewAggregator = new ViewsAggregator(
            new DynamoDBContext(new AmazonDynamoDBClient()),
            new DateTimeProvider());
    }
    public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        await viewAggregator.Handle(dynamoEvent, context);
    }
}