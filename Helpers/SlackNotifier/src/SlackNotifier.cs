using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace SlackNotifier;

public class SlackNotifier
{
    private readonly IAmazonSimpleSystemsManagement ssmClient;
    private readonly ISlackClient slackClient;
    

    public SlackNotifier(IAmazonSimpleSystemsManagement ssmClient, ISlackClient slackClient)
    {
        this.ssmClient = ssmClient;
        this.slackClient = slackClient;
    }
    
    public async Task Handle(SNSEvent @event, ILambdaContext context)
    {
        var slackUrl = await GetSlackUrl();
        
        foreach (var record in @event.Records)
        {
            context.Logger.LogInformation($"Processed record {record.Sns.Message}");

            var alarmMessage = JsonSerializer.Deserialize<AlarmMessage>(record.Sns.Message) ??
                               throw new ApplicationException("Cannot deserialize alarm message");

            if (alarmMessage.IsAlarmState)
            {
                var slackMessage = $":fire: {alarmMessage.AlarmName} state is now {alarmMessage.NewStateValue}: {alarmMessage.NewStateReason}";

                await slackClient.SendMessage(slackMessage, slackUrl);
            }
        }
    }

    private async Task<string> GetSlackUrl()
    {
        var request = new GetParameterRequest()
        {
            Name = "/CloudResumeChallenge/SlackUrl",
            WithDecryption = true
        };

        var result = await ssmClient.GetParameterAsync(request);

        return result.Parameter.Value;
    }
}