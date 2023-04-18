using Amazon.CDK.AWS.Route53;

namespace Build;

public class BackEndProps
{
    public IHostedZone HostedZone { get; set; }
    
    public string FrontEndDomain { get; set; }

    public bool UseDockerBundling { get; set; }
}