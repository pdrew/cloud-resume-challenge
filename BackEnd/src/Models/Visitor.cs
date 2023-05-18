using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace BackEnd.Models;

public class Visitor
{
    public Visitor()
    {
        
    }
    public Visitor(string id)
    {
        PartitionKey = "VISITOR";
        Id = id;
    }
    
    [DynamoDBHashKey("pk")]
    [JsonIgnore]
    public string PartitionKey { get; private set; }
    
    [DynamoDBRangeKey("sk")]
    
    public string Id { get; private set; }
    
    
    [DynamoDBProperty("views")]
    public int Views { get; set; }
}