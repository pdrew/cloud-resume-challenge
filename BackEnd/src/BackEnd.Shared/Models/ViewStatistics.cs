using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace BackEnd.Shared.Models;

public class ViewStatistics
{
    public ViewStatistics()
    {
        
    }
    
    public ViewStatistics(string month)
    {
        PartitionKey = "STATISTICS";
        Month = month;
    }
    
    [DynamoDBHashKey("pk")]
    [JsonIgnore]
    public string? PartitionKey { get; private set; }
    
    [DynamoDBRangeKey("sk")]
    public string? Month { get; private set; }
    
    [DynamoDBProperty("total_views")]
    public int TotalViews { get; set; }
    
    [DynamoDBProperty("unique_visitors")]
    public int UniqueVisitors { get; set; }
}