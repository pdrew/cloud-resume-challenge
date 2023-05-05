using Amazon.CDK;
using Amazon.CDK.AWS.Route53;
using Constructs;

namespace Build
{
    public class CloudResumeChallengeStack : Stack
    {
        internal CloudResumeChallengeStack(Construct scope, string id, CloudResumeChallengeStackProps props) : base(scope, id, props)
        {
            var zone = HostedZone.FromLookup(this, "CloudResumeChallengeHostedZone", new HostedZoneProviderProps()
            {
                DomainName = props.Domain
            });

            new FrontEnd(this, "CloudResumeChallengeFrontEnd", new FrontEndProps()
            {
                Subdomain = props.Subdomain,
                HostedZone = zone,
                Env = props.Env
            });
            
            new BackEnd(this, "CloudResumeChallengeBackEnd", new BackEndProps()
            {
                Subdomain = props.Subdomain,
                HostedZone = zone,
                SlackUrl = props.SlackUrl,
                Env = props.Env
            });
            
            
        }
    }
}
