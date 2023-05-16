using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace BackEnd.Models;

public class Visitor
{
    public Visitor()
    {
        
    }
    public Visitor(string ipHash)
    {
        PartitionKey = $"{nameof(Visitor)}|{ipHash}";
    }
    
    [DynamoDBHashKey("pk")]
    [JsonIgnore]
    public string PartitionKey { get; private set; }
    
    [DynamoDBProperty("views")]
    public int Views { get; set; }

    public bool IsNew() => Views == 0;
}