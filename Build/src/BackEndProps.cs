using Amazon.CDK.AWS.Route53;

namespace Build;

public class BackEndProps
{
    public string Subdomain { get; set; }
    
    public IHostedZone HostedZone { get; set; }

    public string BackEndRecordName => $"{Subdomain}-api";

    public string BackEndDomainName => $"{BackEndRecordName}.{HostedZone.ZoneName}";
    
    public string FrontEndDomainName => $"{Subdomain}.{HostedZone.ZoneName}";
}