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

namespace Build
{
    public class CloudResumeChallengeStack : Stack
    {
        internal CloudResumeChallengeStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var domainName = "dev-resume.patrickdrew.com";
            
            var bucket = new Bucket(this, "CloudResumeChallengeBucket", new BucketProps()
            {
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            new BucketDeployment(this, "CloudResumeChallengeBucketDeployment", new BucketDeploymentProps()
            {
                Sources = new [] { Source.Asset("../FrontEnd/dist")},
                DestinationBucket = bucket
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

            new ARecord(this, "CloudResumeChallengeARecord", new ARecordProps()
            {   
                Zone = zone,
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution))
            });
        }
    }
}
