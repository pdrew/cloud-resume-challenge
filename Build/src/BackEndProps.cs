using Amazon.CDK;
using Amazon.CDK.AWS.Route53;

namespace Build;

public class BackEndProps : StackProps
{
    public string Subdomain { get; init; }
    
    public IHostedZone HostedZone { get; init; }

    public string SlackUrl { get; init; }

    public string BackEndRecordName => $"{Subdomain}-api";

    public string BackEndDomainName => $"{BackEndRecordName}.{HostedZone.ZoneName}";
    
    public string FrontEndDomainName => $"{Subdomain}.{HostedZone.ZoneName}";
}