using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Build
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var subdomain = app.Node.TryGetContext("subdomain")?.ToString()
                             ?? throw new ArgumentException("Must provide context argument for subdomain.");
            
            var domain = app.Node.TryGetContext("domain")?.ToString()
                             ?? throw new ArgumentException("Must provide context argument for domain.");

            new CloudResumeChallengeStack(app, "CloudResumeChallengeStack", new CloudResumeChallengeStackProps()
            {
                Subdomain = subdomain,
                Domain = domain,
                Env = new Amazon.CDK.Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
                }
            });
            
            app.Synth();
        }
    }
}
