using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.Signer;
using Amazon.CDK.CustomResources;
using Constructs;
using AssetOptions = Amazon.CDK.AWS.S3.Assets.AssetOptions;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;
using BundlingOptions = Amazon.CDK.BundlingOptions;
namespace Build;

public class BackEnd : Construct
{
    public BackEnd(Construct scope, string id, bool useDockerBundling, IHostedZone zone) : base(scope, id)
    {
        var subdomainName = $"resume-api.{zone.ZoneName}";
        
        var certificate = new Certificate(this, "Certificate", new CertificateProps()
        {
            DomainName = subdomainName,
            Validation = CertificateValidation.FromDns(zone) 
        });
        
        var table = new Table(this, "DynamoTable", new TableProps()
        {
            TableName = "CloudResumeChallengeDatabase",
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
            // UntrustedArtifactOnDeployment = UntrustedArtifactOnDeployment.ENFORCE
        });
        
        var bucket = new Bucket(this, "Bucket", new BucketProps()
        {
            BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = true,
            Versioned = true
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
        
        new BucketDeployment(this, "BucketDeployment", new BucketDeploymentProps()
        {
            Sources = new [] 
            { 
                Source.Asset("../BackEnd/src", new AssetOptions()
                {
                    Bundling = bundlingOptions
                }) 
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
            Code = Code.FromAsset("../CodeSigner/src/", new AssetOptions()
            {
                Bundling = bundlingOptions
            }),
        });

        var codeSignerProvider = new Provider(this, "CodeSignerProvider", new ProviderProps()
        {
            OnEventHandler = codeSignerFunction
        });

        new CustomResource(this, "CodeSignerResource", new CustomResourceProps()
        {
            ServiceToken = codeSignerProvider.ServiceToken,
            Properties = new Dictionary<string, object>()
            {
                { "BucketArn", bucket.BucketArn },
                { "TimeStamp", DateTimeOffset.Now.ToUnixTimeSeconds() },
            },
        });
        
        var lambdaFunction = new Function(this, "LambdaFunction", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "BackEnd",
            Timeout = Duration.Seconds(30),
            Code = GetFunctionCode(useDockerBundling),
            CodeSigningConfig = signingConfig
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
            
        var api = new LambdaRestApi(this, "Api", new LambdaRestApiProps()
        {
           Handler = lambdaFunction,
           Proxy = true,
           DomainName = new DomainNameOptions()
           {
               DomainName = subdomainName,
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
            RecordName = "resume-api",
            Zone = zone,
            Target = RecordTarget.FromAlias(new ApiGateway(api))
        });
    }
    
    private Code GetFunctionCode(bool useDockerBundling)
    {
        if (!useDockerBundling)
        {
            return Code.FromAsset("../BackEnd/dist/function.zip");
        }
            
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
        
        return Code.FromAsset("../BackEnd/src/", new AssetOptions()
        {
            Bundling = bundlingOptions
        });
    }
}