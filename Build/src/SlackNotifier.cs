using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.CustomResources;
using Constructs;

namespace Build;

public class SlackNotifier  : Construct
{
    public SlackNotifier(Construct scope, string id, SlackNotifierProps props) : base(scope, id)
    {
        var parameterName = "/CloudResumeChallenge/SlackUrl";
        
        var function = new Function(this, "Function", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "SlackNotifier::SlackNotifier.Function::FunctionHandler",
            Timeout = Duration.Seconds(30),
            Code = Code.FromAsset("../Helpers/SlackNotifier/dist/slacknotifier-function.zip"),
            Description = "SlackNotifierFunction"
        });

        var eventSource = new SnsEventSource(props.Topic);
        
        function.AddEventSource(eventSource);

        var sdkCall = new AwsSdkCall()
        {
            Service = "SSM",
            Action = "putParameter",
            Parameters = new Dictionary<string, object> 
            {
                { "Name",  parameterName },
                { "Value",  props.SlackUrl },
                { "Overwrite",  true },
                { "Type",  "SecureString" }
            },
            PhysicalResourceId = PhysicalResourceId.Of($"SlackUrlParameter{scope.Node.Root.Node.Id}")
        };
        
        new AwsCustomResource(this, "SecureParameter", new AwsCustomResourceProps()
        {
            OnCreate = sdkCall,
            OnUpdate = sdkCall,
            OnDelete = new AwsSdkCall()
            {
                Service = "SSM",
                Action = "deleteParameter",
                Parameters = new Dictionary<string, string>
                {
                    { "Name",  parameterName }
                }
            },
            Policy = AwsCustomResourcePolicy.FromSdkCalls(new SdkCallsPolicyOptions()
            {
                Resources = AwsCustomResourcePolicy.ANY_RESOURCE
            })
        });
        
        function.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Sid = "ParameterStoreAccess",
            Effect = Effect.ALLOW,
            Actions = new []
            {
                "ssm:GetParameterHistory",
                "ssm:GetParametersByPath",
                "ssm:GetParameters",
                "ssm:GetParameter",
            },
            Resources = new []
            {
                $"arn:aws:ssm:{props.Env.Region}:{props.Env.Account}:parameter{parameterName}"
            }
        }));
    }
}