namespace CodeSigner;

public class CustomResourceEvent<T>
{
    public string? RequestType { get; set; }
    public string? StackId { get; set; }
    public string? RequestId { get; set; }
    public string? LogicalResourceId { get; set; }
 
    public string? ResourceType { get; set; }
    public string? PhysicalResourceId { get; set; }
    
    public T ResourceProperties { get; set; }
}

public class CustomResourceResponse
{
    public string PhysicalResourceId { get; set; }
    
    public bool NoEcho { get; set; }
    
    public Dictionary<string, object> Data { get; set; }
}