using Amazon.CDK.AWS.Route53;

namespace Build;

public class FrontEndProps
{
    public IHostedZone HostedZone { get; set; }
}