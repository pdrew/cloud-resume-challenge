using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.Signer;

namespace Build;

public class CodeSignerProps
{
    public Bucket Bucket { get; set; }
    
    public BucketDeployment BucketDeployment { get; set; }
    
    public SigningProfile SigningProfile  { get; set; }
}