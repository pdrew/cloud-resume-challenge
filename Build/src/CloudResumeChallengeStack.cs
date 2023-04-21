using System;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Route53;
using Constructs;

namespace Build
{
    public class CloudResumeChallengeStack : Stack
    {
        internal CloudResumeChallengeStack(Construct scope, string id, CloudResumeChallengeStackProps props) : base(scope, id, props)
        {
            var zone = GetHostedZone(props.EnvironmentDescription, props.DomainName);
            
            new FrontEnd(this, "CloudResumeChallengeFrontEnd", new FrontEndProps()
            {
                HostedZone = zone
            });
            
            new BackEnd(this, "CloudResumeChallengeBackEnd", new BackEndProps()
            {
                HostedZone = zone
            });
        }

        private IHostedZone GetHostedZone(string environmentDescription, string domainName)
        {
            if (environmentDescription.Equals("prod", StringComparison.CurrentCultureIgnoreCase))
            {
                return HostedZone.FromLookup(this, "CloudResumeChallengeHostedZone", new HostedZoneProviderProps()
                {
                    DomainName = domainName
                });
            }
            
            var zone = new PublicHostedZone(this, "CloudResumeChallengeHostedZone", new PublicHostedZoneProps()
            {
                ZoneName = $"{environmentDescription.ToLower()}.{domainName.ToLower()}"
            });
            
            var editorRole = Role.FromRoleArn(this, "ParentHostedZoneEditorRole",
                "arn:aws:iam::194453828363:role/CloudResumeChallengeHostedZoneEditorRole");
            
            new CrossAccountZoneDelegationRecord(this, "CloudResumeChallengeZoneDelegationRecord", 
                new CrossAccountZoneDelegationRecordProps()
                {
                    DelegatedZone = zone,
                    ParentHostedZoneName = domainName,
                    DelegationRole = editorRole
                });

            return zone;
        }
    }
}
