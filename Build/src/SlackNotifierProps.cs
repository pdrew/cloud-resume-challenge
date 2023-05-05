using Amazon.CDK;
using Amazon.CDK.AWS.SNS;

namespace Build;

public class SlackNotifierProps : StackProps
{
    public Topic Topic { get; init; }
    
    public string SlackUrl { get; init; }
}