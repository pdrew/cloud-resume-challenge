using Amazon.CDK.AWS.Glue;
using Amazon.CDK.AWS.Route53;

namespace Build;

public class FrontEndProps
{
    public string Subdomain { get; set; }
    
    public IHostedZone HostedZone { get; set; }

    public string FrontEndDomainName => $"{Subdomain}.{HostedZone.ZoneName}";
}