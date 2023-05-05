using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.TestUtilities;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Moq;
using Xunit;

namespace SlackNotifier.Tests;

public class SlackNotifierTests
{
    private readonly Mock<IAmazonSimpleSystemsManagement> ssmClientMock = new();
    private readonly Mock<ISlackClient> slackClientMock = new();

    [Fact]
    public async Task HandleSendsSlackMessage()
    {
        var context = new TestLambdaContext();

        var snsEvent = new SNSEvent
        {
            Records = new List<SNSEvent.SNSRecord>
            {
                new SNSEvent.SNSRecord
                {
                    Sns = new SNSEvent.SNSMessage()
                    {
                        Message = "{ \"AlarmName\": \"Foo\", \"NewStateValue\": \"ALARM\", \"NewStateReason\": \"Bar\" }"
                    }
                }
            }
        };

        ssmClientMock
            .Setup(x => x.GetParameterAsync(
                It.IsAny<GetParameterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetParameterResponse()
            {
                Parameter = new Parameter()
                {
                    Value = "https://foo.bar.baz"
                }
            });
        
        slackClientMock.Setup(x => x.SendMessage(It.IsAny<string>(), "https://foo.bar.baz")).Verifiable();

        var sut = new SlackNotifier(ssmClientMock.Object, slackClientMock.Object);

        await sut.Handle(snsEvent, context);
        
        slackClientMock.Verify();
    }
}