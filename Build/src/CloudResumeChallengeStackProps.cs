using Amazon.CDK;

namespace Build;

public class CloudResumeChallengeStackProps : StackProps
{
    public string Subdomain  { get; init; }
    public string Domain { get; init; }
    
    public string SlackUrl { get; init; } 
}