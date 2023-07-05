using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace BackEnd.Shared.Models;

public class Visitor
{
    public Visitor()
    {
        
    }
    public Visitor(string id, long expiration)
    {
        PartitionKey = "VISITOR";
        Id = id;
        Expiration = expiration;
    }
    
    [DynamoDBHashKey("pk")]
    [JsonIgnore]
    public string? PartitionKey { get; private set; }
    
    [DynamoDBRangeKey("sk")]
    
    public string? Id { get; private set; }
    
    
    [DynamoDBProperty("total_views")]
    public int? TotalViews { get; set; }
    
    [DynamoDBProperty("ttl")]
    public long? Expiration  { get; private set; }
    
}