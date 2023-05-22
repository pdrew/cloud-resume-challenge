using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace ViewsAggregator.Models;

public class ViewStatistics
{
    public ViewStatistics()
    {
        PartitionKey = "STATISTICS";
        SortKey = "VIEWS";
    }
    
    [DynamoDBHashKey("pk")]
    public string PartitionKey { get; set; }
    
    [DynamoDBRangeKey("sk")]
    public string SortKey { get; set; }
    
    [DynamoDBProperty("total_views")]
    public int TotalViews { get; set; }
    
    [DynamoDBProperty("unique_visitors")]
    public int UniqueVisitors { get; set; }
}