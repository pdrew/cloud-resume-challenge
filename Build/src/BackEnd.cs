using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;
using AssetOptions = Amazon.CDK.AWS.S3.Assets.AssetOptions;

namespace Build;

public class BackEnd : Construct
{
    public BackEnd(Construct scope, string id, bool useDockerBundling) : base(scope, id)
    {
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
            
            var lambdaFunction = new Function(this, "CloudResumeChallengeLambdaFunction", new FunctionProps()
            {
                Runtime = Runtime.DOTNET_6,
                MemorySize = 256,
                LogRetention = RetentionDays.ONE_DAY,
                Handler = "BackEnd",
                Timeout = Duration.Seconds(30),
                Code = GetFunctionCode(useDockerBundling)
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
            
            new LambdaRestApi(this, "CloudResumeChallengeApi", new LambdaRestApiProps()
            {
               Handler = lambdaFunction,
               Proxy = true
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