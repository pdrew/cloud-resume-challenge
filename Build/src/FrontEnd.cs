using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Constructs;
using AssetOptions = Amazon.CDK.AWS.S3.Assets.AssetOptions;

namespace Build;

public class FrontEnd : Construct
{
    public FrontEnd(Construct scope, string id, bool useDockerBundling) : base(scope, id)
    {
        var domainName = "dev-resume.patrickdrew.com";
        
        var bucket = new Bucket(this, "CloudResumeChallengeBucket", new BucketProps()
            {
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                RemovalPolicy = RemovalPolicy.DESTROY
            });
            
            var zone = new PublicHostedZone(this, "CloudResumeChallengeHostedZone", new PublicHostedZoneProps()
            {
                ZoneName = domainName
            });

            var editorRole = Role.FromRoleArn(this, "ParentHostedZoneEditorRole",
                "arn:aws:iam::586696426658:role/CloudResumeChallengeHostedZoneEditorRole");

            new CrossAccountZoneDelegationRecord(this, "CloudResumeChallengeZoneDelegationRecord", 
                new CrossAccountZoneDelegationRecordProps()
                {
                    DelegatedZone = zone,
                    ParentHostedZoneName = "patrickdrew.com",
                    DelegationRole = editorRole
                });
            
            var certificate = new DnsValidatedCertificate(this, "CloudResumeChallengeCertificate",
                new DnsValidatedCertificateProps()
                {
                    DomainName = domainName,
                    HostedZone = zone,
                    Region = "us-east-1"
                });

            var distribution = new Distribution(this, "CloudResumeChallengeDistribution", new DistributionProps()
            {
                DefaultRootObject = "index.html",
                DefaultBehavior = new BehaviorOptions()
                {
                    Origin = new S3Origin(bucket),
                    ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS
                },
                DomainNames = new []{ domainName },
                Certificate = certificate
            });

            new BucketDeployment(this, "CloudResumeChallengeBucketDeployment", new BucketDeploymentProps()
            {
                Sources = new [] { GetBucketSource(useDockerBundling) },
                DestinationBucket = bucket,
                Distribution = distribution,
            });

            new ARecord(this, "CloudResumeChallengeARecord", new ARecordProps()
            {   
                Zone = zone,
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution))
            });
    }
    
    
    private ISource GetBucketSource(bool useDockerBundling)
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
            }
        };

        return Source.Asset("../FrontEnd/src", new AssetOptions()
        {
            Bundling = bundlingOptions
        });
    }
}