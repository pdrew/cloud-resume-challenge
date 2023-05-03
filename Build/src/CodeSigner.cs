using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.CustomResources;
using Constructs;

namespace Build;

public class CodeSigner  : Construct
{
    public string SignedObjecKey { get; }
    
    public CodeSigner(Construct scope, string id, CodeSignerProps props) : base(scope, id)
    {
        
        var codeSignerFunction = new Function(this, "Function", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "CodeSigner::CodeSigner.Function::FunctionHandler",
            Timeout = Duration.Seconds(30),
            Code = Code.FromAsset("../Helpers/CodeSigner/dist/codesigner-function.zip"),
            Description = "CodeSignerFunction"
        });
        
        codeSignerFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Sid = "SignerAccess",
            Effect = Effect.ALLOW,
            Actions = new []
            {
                "signer:StartSigningJob",
                "signer:DescribeSigningJob"
            },
            Resources = new []
            {
                "*"
            }
        }));
        
        codeSignerFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Sid = "S3ObjectVersionAccess",
            Effect = Effect.ALLOW,
            Actions = new []
            {
                "s3:ListBucketVersions",
                "s3:GetObjectVersion",
                "s3:PutObject",
                "s3:ListBucket"
            },
            Resources = new []
            {
                props.Bucket.BucketArn,
                $"{props.Bucket.BucketArn}/*"
            }
        }));

        var codeSignerProvider = new Provider(this, "Provider", new ProviderProps()
        {
            OnEventHandler = codeSignerFunction
        });
        
        var codeSignerResource = new CustomResource(this, "CustomResource", new CustomResourceProps()
        {
            ServiceToken = codeSignerProvider.ServiceToken,
            Properties = new Dictionary<string, object>()
            {
                { "ProfileName", props.SigningProfile.SigningProfileName },
                { "BucketName", props.Bucket.BucketName },
                { "ObjectKey", $"Unsigned/{Fn.Select(0, props.BucketDeployment.ObjectKeys)}" },                
                { "TimeStamp", DateTimeOffset.Now.ToUnixTimeSeconds() },
            },
        });
        
        SignedObjecKey = codeSignerResource.GetAttString("Key");
    }
}