using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Route53;
using Constructs;

namespace Build
{
    public class CloudResumeChallengeStack : Stack
    {
        internal CloudResumeChallengeStack(Construct scope, string id, bool useDockerBundling, IStackProps props = null) : base(scope, id, props)
        {
            var domainName = "test.patrickdrew.com";
            
            var zone = new PublicHostedZone(this, "CloudResumeChallengeHostedZone", new PublicHostedZoneProps()
            {
                ZoneName = domainName
            });
            
            var editorRole = Role.FromRoleArn(this, "ParentHostedZoneEditorRole",
                "arn:aws:iam::194453828363:role/CloudResumeChallengeHostedZoneEditorRole");
            
            new CrossAccountZoneDelegationRecord(this, "CloudResumeChallengeZoneDelegationRecord", 
                new CrossAccountZoneDelegationRecordProps()
                {
                    DelegatedZone = zone,
                    ParentHostedZoneName = "patrickdrew.com",
                    DelegationRole = editorRole
                });
            
            new BackEnd(this, "CloudResumeChallengeBackEnd", useDockerBundling, zone);
            
            new FrontEnd(this, "CloudResumeChallengeFrontEnd", useDockerBundling, zone);
        }
    }
}
