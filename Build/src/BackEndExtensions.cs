using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.CloudWatch.Actions;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SNS;
using Constructs;

namespace Build;

public static class BackEndExtensions
{
    public static void AddDynamoPolicies(this Function function, Table table)
    {
        function.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
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
            
        function.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
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
            
        function.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
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
    }

    public static void AddAlarms(this LambdaRestApi api, Construct scope, Topic topic)
    {
        api.MetricLatency(new MetricOptions()
            {
                Statistic = "Average",
                Period = Duration.Minutes(5)
            })
            .CreateAlarm(scope, "ApiLatencyAlarm", new CreateAlarmOptions()
            {
                ComparisonOperator = ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
                Threshold = 2000,
                EvaluationPeriods = 1
            })
            .AddAlarmAction(new SnsAction(topic));
        
        api.MetricCount(new MetricOptions()
            {
                Statistic = "Sum",
                Period = Duration.Minutes(5)
            })
            .CreateAlarm(scope, "ApiCountAlarm", new CreateAlarmOptions()
            {
                ComparisonOperator = ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
                Threshold = 500,
                EvaluationPeriods = 1
            })
            .AddAlarmAction(new SnsAction(topic));
        
        api.MetricServerError(new MetricOptions()
            {
                Statistic = "Sum",
                Period = Duration.Minutes(5)
            })
            .CreateAlarm(scope, "ApiServerErrorAlarm", new CreateAlarmOptions()
            {
                ComparisonOperator = ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
                Threshold = 1,
                EvaluationPeriods = 1
            })
            .AddAlarmAction(new SnsAction(topic));
    }

    public static void AddAlarms(this Function function, Construct scope, Topic topic)
    {
        function.MetricThrottles(new MetricOptions()
            {
                Statistic = "Sum",
                Period = Duration.Minutes(5)
            })
            .CreateAlarm(scope, "ApiFunctionThrottleAlarm", new CreateAlarmOptions()
            {
                ComparisonOperator = ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
                Threshold = 1,
                EvaluationPeriods = 1
            })
            .AddAlarmAction(new SnsAction(topic));
    }
}