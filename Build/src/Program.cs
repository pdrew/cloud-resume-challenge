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

            var environment = app.Node.TryGetContext("environment")?.ToString()
                ?? throw new ArgumentException("Must provide context argument for environment.");

            var domainName = app.Node.TryGetContext("domain")?.ToString()
                ?? throw new ArgumentException("Must provide context argument for domain.");

            new CloudResumeChallengeStack(app, "CloudResumeChallengeStack", new CloudResumeChallengeStackProps()
            {
                EnvironmentDescription = environment,
                DomainName = domainName,
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
