using Amazon.CDK;

namespace Build;

public class CloudResumeChallengeStackProps : StackProps
{
    public string Subdomain  { get; set; }

    public string Domain { get; set; }
}