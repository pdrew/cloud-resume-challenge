using Amazon.CDK;

namespace Build;

public class CloudResumeChallengeStackProps : StackProps
{
    public string EnvironmentDescription  { get; set; }
    
    public string DomainName { get; set; }
}