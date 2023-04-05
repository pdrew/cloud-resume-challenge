using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Assets;
using Amazon.CDK.AWS.S3.Deployment;
using Constructs;
using AssetOptions = Amazon.CDK.AWS.S3.Assets.AssetOptions;
using Function = Amazon.CDK.AWS.Lambda.Function;
using FunctionProps = Amazon.CDK.AWS.Lambda.FunctionProps;

namespace Build
{
    public class CloudResumeChallengeStack : Stack
    {
        internal CloudResumeChallengeStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            //FrontEnd
            
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
                Sources = new [] { Source.Asset("../FrontEnd/out")},
                DestinationBucket = bucket,
                Distribution = distribution,
            });

            
            new ARecord(this, "CloudResumeChallengeARecord", new ARecordProps()
            {   
                Zone = zone,
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution))
            });
            
            //BackEnd
            var table = new Table(this, "CloudResumeChallengeDatabase", new TableProps()
            {
                TableName = "CloudResumeChallengeDatabase",
                PartitionKey = new Attribute()
                {
                    Name = "pk",
                    Type = AttributeType.STRING
                },
                RemovalPolicy = RemovalPolicy.DESTROY
            });
            
            var bundlingOptions = new BundlingOptions()
            {
                Image = Runtime.DOTNET_6.BundlingImage,
                User = "root",
                OutputType = BundlingOutput.ARCHIVED,
                Command = new []
                {
                    "/bin/sh",
                    "-c",
                    "dotnet tool install -g Amazon.Lambda.Tools" +
                    " && dotnet build" +
                    " && dotnet lambda package --output-package /asset-output/function.zip"
                }
            };

            var lambdaFunction = new Function(this, "CloudResumeChallengeLambdaFunction", new FunctionProps()
            {
                Runtime = Runtime.DOTNET_6,
                MemorySize = 256,
                LogRetention = RetentionDays.ONE_DAY,
                Handler = "BackEnd",
                Timeout = Duration.Seconds(30),
                Code = Code.FromAsset("../BackEnd/src/", new AssetOptions()
                {
                    Bundling = bundlingOptions
                })
            });
            
            lambdaFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
            {
                Sid = "DynamoDBIndexAndStreamAccess",
                Effect = Effect.ALLOW,
                Actions = new []
                {
                    "dynamodb:GetShardIterator",
                    "dynamodb:Scan",
                    "dynamodb:Query",
                    "dynamodb:DescribeStream",
                    "dynamodb:GetRecords",
                    "dynamodb:ListStreams"
                },
                Resources = new []
                {
                    table.TableArn,
                    $"{table.TableArn}/stream/*"
                }
            }));
            
            lambdaFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
            {
                Sid = "DynamoDBTableAccess",
                Effect = Effect.ALLOW,
                Actions = new []
                {
                    "dynamodb:BatchGetItem",
                    "dynamodb:BatchWriteItem",
                    "dynamodb:ConditionCheckItem",
                    "dynamodb:PutItem",
                    "dynamodb:DescribeTable",
                    "dynamodb:DeleteItem",
                    "dynamodb:GetItem",
                    "dynamodb:Scan",
                    "dynamodb:Query",
                    "dynamodb:UpdateItem"
                },
                Resources = new []
                {
                    table.TableArn
                }
            }));
            
            lambdaFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
            {
                Sid = "DynamoDBDescribeLimitsAccess",
                Effect = Effect.ALLOW,
                Actions = new []
                {
                    "dynamodb:DescribeLimits"
                },
                Resources = new []
                {
                    table.TableArn,
                    $"{table.TableArn}/stream/*"
                }
            }));
            
            var restApi = new LambdaRestApi(this, "CloudResumeChallengeApi", new LambdaRestApiProps()
            {
               Handler = lambdaFunction,
               Proxy = true
            });
            
            new CfnOutput(this, "ApiGatewayArn", new CfnOutputProps() { Value = restApi.ArnForExecuteApi() });
        }
    }
}
