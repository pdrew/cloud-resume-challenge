using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.Signer;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace Build;

public class BackEnd : Construct
{
    public BackEnd(Construct scope, string id, BackEndProps props) : base(scope, id)
    {
        var certificate = new Certificate(this, "Certificate", new CertificateProps()
        {
            DomainName = props.BackEndDomainName,
            Validation = CertificateValidation.FromDns(props.HostedZone) 
        });
        
        var table = new Table(this, "DynamoTable", new TableProps()
        {
            PartitionKey = new Attribute()
            {
                Name = "pk",
                Type = AttributeType.STRING
            },
            SortKey = new Attribute()
            {
                Name = "sk",
                Type = AttributeType.STRING
            },
            RemovalPolicy = RemovalPolicy.DESTROY,
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            TimeToLiveAttribute = "ttl"
        });

        new ViewsAggregator(this, "ViewsAggregator", new ViewsAggregatorProps()
        {
            Table = table
        });

        var signingProfile = new SigningProfile(this, "SigningProfile", new SigningProfileProps()
        {   
            Platform = Platform.AWS_LAMBDA_SHA384_ECDSA
        });

        var signingConfig = new CodeSigningConfig(this, "CodeSigningConfig", new CodeSigningConfigProps()
        {
            SigningProfiles = new [] { signingProfile },
            UntrustedArtifactOnDeployment = UntrustedArtifactOnDeployment.ENFORCE
        });
        
        var bucket = new Bucket(this, "Bucket", new BucketProps()
        {
            BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = true,
            Versioned = true
        });
        
        var bucketDeployment = new BucketDeployment(this, "BucketDeployment", new BucketDeploymentProps()
        {
            Sources = new []
            { 
                Source.Asset("../BackEnd/dist/backend-function.zip") 
            },
            DestinationBucket = bucket,
            DestinationKeyPrefix = "Unsigned",
            Extract = false
        });

        var codeSigner = new CodeSigner(this, "CodeSigner", new CodeSignerProps()
        {
            Bucket = bucket,
            BucketDeployment = bucketDeployment,
            SigningProfile = signingProfile,
            Env = props.Env
        });
        
        var lambdaFunction = new Function(this, "ApiFunction", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "BackEnd.Api",
            Timeout = Duration.Seconds(30),
            Code = Code.FromBucket(bucket, codeSigner.SignedObjecKey),
            CodeSigningConfig = signingConfig,
            Description = "ApiFunction",
            Environment = new Dictionary<string, string>
            {
                { "DYNAMODB_TABLE", table.TableName },
                { "FRONTEND_DOMAIN",  props.FrontEndDomainName }
            }
        });
        
        lambdaFunction.AddDynamoPolicies(table);
            
        var api = new LambdaRestApi(this, "Api", new LambdaRestApiProps()
        {
           Handler = lambdaFunction,
           Proxy = true,
           DomainName = new DomainNameOptions()
           {
               DomainName = props.BackEndDomainName,
               Certificate = certificate
           },
           DeployOptions = new StageOptions()
           {
               MethodOptions = new Dictionary<string, IMethodDeploymentOptions>()
               {
                   {
                       "/*/*", new MethodDeploymentOptions()
                       {
                           ThrottlingBurstLimit = 1000,
                           ThrottlingRateLimit = 10
                       }
                   }
               }
           }
        });
        
        new ARecord(this, "ARecord", new ARecordProps()
        {
            RecordName = props.BackEndRecordName,
            Zone = props.HostedZone,
            Target = RecordTarget.FromAlias(new ApiGateway(api))
        });
        
        new CfnOutput(this, "Url", new CfnOutputProps() { Value = $"https://{props.BackEndDomainName}" });
        
        var topic = new Topic(this, "Topic");

        topic.AddSubscription(new EmailSubscription("patrick.r.drew+crc-alarms@gmail.com"));
        
        api.AddAlarms(this, topic);
        
        lambdaFunction.AddAlarms(this, topic);

        new SlackNotifier(this, "SlackNotifier", new SlackNotifierProps()
        {
            Topic = topic,
            SlackUrl = props.SlackUrl,
            Subdomain = props.Subdomain,
            Env = props.Env
        });
    }
}