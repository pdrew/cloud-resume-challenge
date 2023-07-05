namespace CodeSigner;

public class SignLambdaRequest
{
    public string? ProfileName { get; set; }
    public string? BucketName { get; set; }
    public string? ObjectKey { get; set; }
    
    public string? TimeStamp { get; set; }
}