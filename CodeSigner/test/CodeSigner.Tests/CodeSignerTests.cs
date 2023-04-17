using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Signer;
using Amazon.Signer.Model;
using Moq;

namespace CodeSigner.Tests;

public class FunctionTest
{
    private Mock<IAmazonSigner> signerClientMock = new ();
    private Mock<IAmazonS3> s3ClientMock = new ();

    [Fact]
    public async Task HandleReturnsSignedObjectBucketDetails()
    {
        var context = new TestLambdaContext();

        var customResourceEvent = new CustomResourceEvent<SignLambdaRequest>()
        {
            RequestType = "Create",
            ResourceProperties = new SignLambdaRequest()
            {
                ProfileName = "Foo",
                BucketName = "Bar",
                ObjectKey = "Baz"
            }
        };
        
        s3ClientMock
            .Setup(x => x.ListVersionsAsync(
                    It.IsAny<ListVersionsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ListVersionsResponse()
                {
                    Versions = new List<S3ObjectVersion>()
                    {
                        new () { IsLatest = true, VersionId = "Qux" } 
                    }
                });

        signerClientMock
            .Setup(x => x.StartSigningJobAsync(
                It.IsAny<StartSigningJobRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new StartSigningJobResponse()
                {
                    JobId = "Grault"
                });

        signerClientMock
            .Setup(x => x.DescribeSigningJobAsync(
                It.IsAny<DescribeSigningJobRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new DescribeSigningJobResponse()
                {
                    Status = SigningStatus.Succeeded,
                    SignedObject = new SignedObject()
                    {
                        S3 = new S3SignedObject()
                        {
                            BucketName = customResourceEvent.ResourceProperties.BucketName,
                            Key = "Garply"
                        }
                    }
                });
        
        var sut = new CodeSigner(signerClientMock.Object, s3ClientMock.Object);

        var actual = await sut.Handle(customResourceEvent, context);

        var actualBucketName = (string)actual.Data.GetValueOrDefault("BucketName");
        var actualKey = (string)actual.Data.GetValueOrDefault("Key");
        
        Assert.Equal(customResourceEvent.ResourceProperties.BucketName, actualBucketName);
        Assert.Equal("Garply", actualKey);
    }
}
