using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace BackEnd.Models;

public class ViewStatistics
{
    public ViewStatistics()
    {
        PartitionKey = nameof(ViewStatistics);
    }
    
    [DynamoDBHashKey("pk")]
    [JsonIgnore]
    public string PartitionKey { get; private set; }
    
    [DynamoDBProperty("total")]
    public int Total { get; set; }
}