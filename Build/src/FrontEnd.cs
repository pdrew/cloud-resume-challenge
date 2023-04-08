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
    public FrontEnd(Construct scope, string id, bool useDockerBundling, IHostedZone zone) : base(scope, id)
    {
        var subdomainName = $"resume.{zone.ZoneName}"; 
        
        var certificate = new Certificate(this, "CloudResumeChallengeFrontEndCertificate", new CertificateProps()
        {
            DomainName = subdomainName,
            Validation = CertificateValidation.FromDns(zone) 
        });
        
        var bucket = new Bucket(this, "CloudResumeChallengeBucket", new BucketProps()
        {
            BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            RemovalPolicy = RemovalPolicy.DESTROY
        });
        
        var distribution = new Distribution(this, "CloudResumeChallengeDistribution", new DistributionProps()
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

        new BucketDeployment(this, "CloudResumeChallengeBucketDeployment", new BucketDeploymentProps()
        {
            Sources = new [] { GetBucketSource(useDockerBundling, zone.ZoneName) },
            DestinationBucket = bucket,
            Distribution = distribution,
        });

        new ARecord(this, "CloudResumeChallengeFrontEndARecord", new ARecordProps()
        {
            RecordName = "resume",
            Zone = zone,
            Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution))
        });
    }
    
    private ISource GetBucketSource(bool useDockerBundling, string domainName)
    {
        if (!useDockerBundling)
        {
            return Source.Asset("../FrontEnd/dist/");
        }
            
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
                { "API_DOMAIN", $"resume-api.{domainName}" }
            }
        };

        return Source.Asset("../FrontEnd/src", new AssetOptions()
        {
            Bundling = bundlingOptions
        });
    }
}