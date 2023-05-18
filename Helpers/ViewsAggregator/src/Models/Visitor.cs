using Amazon.DynamoDBv2.DataModel;

namespace ViewsAggregator.Models;

public class Visitor
{
    [DynamoDBHashKey("pk")]
    public string PartitionKey { get; set; }
    
    [DynamoDBRangeKey("sk")]
    public string SortKey { get; set; }
    
    [DynamoDBProperty("views")]
    public int Views { get; set; }
}