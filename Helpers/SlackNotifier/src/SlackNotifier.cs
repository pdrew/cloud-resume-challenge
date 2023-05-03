using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;

namespace SlackNotifier;

public class SlackNotifier
{
    public async Task Handle(SNSEvent @event, ILambdaContext context)
    {
        foreach (var record in @event.Records)
        {
            context.Logger.LogInformation($"Processed record {record.Sns.Message}");
            
            // TODO: Do interesting work based on the new message
            await Task.CompletedTask;
        }
    }
}