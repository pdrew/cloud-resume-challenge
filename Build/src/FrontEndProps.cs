using Amazon.CDK;
using Amazon.CDK.AWS.Route53;

namespace Build;

public class FrontEndProps : StackProps
{
    public string Subdomain { get; init; }
    
    public IHostedZone HostedZone { get; init; }

    public string FrontEndDomainName => $"{Subdomain}.{HostedZone.ZoneName}";
}