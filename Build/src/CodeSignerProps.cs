using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.Signer;

namespace Build;

public class CodeSignerProps : StackProps
{
    public Bucket Bucket { get; init; }
    
    public BucketDeployment BucketDeployment { get; init; }
    
    public SigningProfile SigningProfile  { get; init; }
}