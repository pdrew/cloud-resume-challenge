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
        PartitionKey = "VISITOR";
        SortKey = ipHash;
    }
    
    [DynamoDBHashKey("pk")]
    public string PartitionKey { get; private set; }
    
    [DynamoDBRangeKey("sk")]
    
    public string SortKey { get; private set; }
    
    
    [DynamoDBProperty("views")]
    public int Views { get; set; }

    public bool IsNew() => Views == 0;
}