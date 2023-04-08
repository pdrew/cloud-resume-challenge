using Amazon.CDK;
using Constructs;

namespace Build
{
    public class CloudResumeChallengeStack : Stack
    {
        internal CloudResumeChallengeStack(Construct scope, string id, bool useDockerBundling, IStackProps props = null) : base(scope, id, props)
        {
            new FrontEnd(this, "CloudResumeChallengeFrontEnd", useDockerBundling);   
                
            new BackEnd(this, "CloudResumeChallengeBackEnd", useDockerBundling);
        }
    }
}
