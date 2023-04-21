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
using AssetOptions = Amazon.CDK.AWS.S3.Assets.AssetOptions;

namespace Build;

public class FrontEnd : Construct
{
    
    
    public FrontEnd(Construct scope, string id, FrontEndProps props) : base(scope, id)
    {
        var subdomainName = $"resume.{props.HostedZone.ZoneName}"; 
        
        var certificate = new Certificate(this, "Certificate", new CertificateProps()
        {
            DomainName = subdomainName,
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
            DomainNames = new []{ subdomainName },
            Certificate = certificate
        });
        
        var bundlingOptions = new BundlingOptions()
        {
            Image = DockerImage.FromRegistry("node:lts"),
            User = "root",
            OutputType = BundlingOutput.NOT_ARCHIVED,
            Command = new []
            {
                "/bin/sh",
                "-c",
                "npm ci" +
                " && npm run build" +
                " && cp -r /asset-input/out/* /asset-output/"
            },
            Environment = new Dictionary<string, string>()
            {
                { "API_DOMAIN", $"resume-api.{props.HostedZone.ZoneName}" }
            }
        };
        
        new BucketDeployment(this, "BucketDeployment", new BucketDeploymentProps()
        {
            Sources = new [] 
            { 
                Source.Asset("../FrontEnd/src", new AssetOptions()
                {
                    Bundling = bundlingOptions
                }) 
            },
            DestinationBucket = bucket,
            Distribution = distribution,
        });

        new ARecord(this, "ARecord", new ARecordProps()
        {
            RecordName = "resume",
            Zone = props.HostedZone,
            Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution))
        });
    }
}