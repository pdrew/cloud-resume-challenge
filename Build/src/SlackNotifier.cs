using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SNS;
using Constructs;

namespace Build;

public class SlackNotifier  : Construct
{
    public SlackNotifier(Construct scope, string id, Topic topic) : base(scope, id)
    {
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

        var eventSource = new SnsEventSource(topic);
        
        function.AddEventSource(eventSource);
    }
}