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
    
    [DynamoDBProperty("total_views")]
    public int TotalViews { get; set; }
    
    [DynamoDBProperty("unique_visitors")]
    public int UniqueVisitors { get; set; }
}