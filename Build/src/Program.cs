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

            if (!new[] { "prod", "test" }.Any(x => environment.Equals(x, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new ArgumentException("environment must be either prod or test");
            }
            
            var subdomain = app.Node.TryGetContext("subdomain")?.ToString()
                             ?? throw new ArgumentException("Must provide context argument for subdomain.");
            
            var domain = app.Node.TryGetContext("domain")?.ToString()
                             ?? throw new ArgumentException("Must provide context argument for domain.");

            var env = environment.Equals("prod", StringComparison.CurrentCultureIgnoreCase)
                ? new Amazon.CDK.Environment
                {
                    Account = "194453828363",
                    Region = "us-east-1"
                }
                : new Amazon.CDK.Environment
                {
                    Account = "428421847827",
                    Region = "us-east-1"
                };

            var stackId =
                $"CloudResumeChallengeStack{(environment.Equals("prod", StringComparison.CurrentCultureIgnoreCase) ? string.Empty : subdomain.ToUpper())}"; 
            
            new CloudResumeChallengeStack(app, stackId, new CloudResumeChallengeStackProps()
            {
                Subdomain = subdomain,
                Domain = domain,
                Env = env
            });
            
            app.Synth();
        }
    }
}
