using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.Util;
using BackEnd.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
                .WithOrigins(
                    $"https://{Environment.GetEnvironmentVariable("FRONTEND_DOMAIN")}"
                )
                .WithMethods(
                    "GET", 
                    "POST"
                )
                .AllowAnyHeader();
        });
});

var chain = new CredentialProfileStoreChain();

if(chain.TryGetAWSCredentials("crc-dev", out var credentials))
{
    var awsOptions = new AWSOptions()
    {
        Credentials = credentials,
        Region = RegionEndpoint.USEast1
    };
    
    builder.Services.AddDefaultAWSOptions(awsOptions);
} 
else 
{ 
    var awsOptions = builder.Configuration.GetAWSOptions();
    
    builder.Services.AddDefaultAWSOptions(awsOptions);
}

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();

AWSConfigsDynamoDB.Context.AddMapping(new TypeMapping(
    typeof(ViewStatistics), Environment.GetEnvironmentVariable("DYNAMODB_TABLE")));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();