using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.CloudWatch.Actions;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.Signer;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Amazon.CDK.CustomResources;
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
            RemovalPolicy = RemovalPolicy.DESTROY
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

        var codeSignerFunction = new Function(this, "CodeSignerFunction", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "CodeSigner::CodeSigner.Function::FunctionHandler",
            Timeout = Duration.Seconds(30),
            Code = Code.FromAsset("../CodeSigner/dist/codesigner-function.zip"),
            Description = "CodeSignerFunction"
        });
        
        codeSignerFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Sid = "SignerAccess",
            Effect = Effect.ALLOW,
            Actions = new []
            {
                "signer:StartSigningJob",
                "signer:DescribeSigningJob"
            },
            Resources = new []
            {
                "*"
            }
        }));
        
        codeSignerFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Sid = "S3ObjectVersionAccess",
            Effect = Effect.ALLOW,
            Actions = new []
            {
                "s3:ListBucketVersions",
                "s3:GetObjectVersion",
                "s3:PutObject",
                "s3:ListBucket"
            },
            Resources = new []
            {
                bucket.BucketArn,
                $"{bucket.BucketArn}/*"
            }
        }));

        var codeSignerProvider = new Provider(this, "CodeSignerProvider", new ProviderProps()
        {
            OnEventHandler = codeSignerFunction
        });
        
        var codeSignerResource = new CustomResource(this, "CodeSignerResource", new CustomResourceProps()
        {
            ServiceToken = codeSignerProvider.ServiceToken,
            Properties = new Dictionary<string, object>()
            {
                { "ProfileName", signingProfile.SigningProfileName },
                { "BucketName", bucket.BucketName },
                { "ObjectKey", $"Unsigned/{Fn.Select(0, bucketDeployment.ObjectKeys)}" },                
                { "TimeStamp", DateTimeOffset.Now.ToUnixTimeSeconds() },
            },
        });

        var signedObjectKey = codeSignerResource.GetAttString("Key");

        var lambdaFunction = new Function(this, "ApiFunction", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "BackEnd",
            Timeout = Duration.Seconds(30),
            Code = Code.FromBucket(bucket, signedObjectKey),
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
        
        api.AddApiAlarms(this, topic);
    }
}