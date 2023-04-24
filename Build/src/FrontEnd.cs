using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Constructs;

namespace Build;

public class FrontEnd : Construct
{
    
    
    public FrontEnd(Construct scope, string id, FrontEndProps props) : base(scope, id)
    {
        var certificate = new Certificate(this, "Certificate", new CertificateProps()
        {
            DomainName = props.FrontEndDomainName,
            Validation = CertificateValidation.FromDns(props.HostedZone) 
        });
        
        var bucket = new Bucket(this, "Bucket", new BucketProps()
        {
            BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = true
        });
        
        var distribution = new Distribution(this, "Distribution", new DistributionProps()
        {
            DefaultRootObject = "index.html",
            DefaultBehavior = new BehaviorOptions()
            {
                Origin = new S3Origin(bucket),
                ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS
            },
            DomainNames = new []{ props.FrontEndDomainName },
            Certificate = certificate
        });
        
        new BucketDeployment(this, "BucketDeployment", new BucketDeploymentProps()
        {
            Sources = new [] 
            { 
                Source.Asset("../FrontEnd/dist") 
            },
            DestinationBucket = bucket,
            Distribution = distribution,
        });

        new ARecord(this, "ARecord", new ARecordProps()
        {
            RecordName = props.Subdomain,
            Zone = props.HostedZone,
            Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution))
        });
        
        new CfnOutput(this, "Url", new CfnOutputProps() { Value = $"https://{props.FrontEndDomainName}" });
    }
}