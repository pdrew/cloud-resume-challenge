using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.TestUtilities;
using BackEnd.Shared.Models;
using BackEnd.Shared.Services;
using Moq;
using Xunit;

namespace ViewsAggregator.Tests;

public class ViewsAggregatorTests
{
    private Mock<IDynamoDBContext> dbMock = new();
    private Mock<IDateTimeProvider> dateTimeProviderMock = new();
    
    private Dictionary<string, AttributeValue> oldImage = new()
    {
        { "PartitionKey", new AttributeValue { S = "VISITOR" } },
        { "Id", new AttributeValue { S = "FOO" } },
        { "TotalViews", new AttributeValue { N = "42" } },
        { "Expiration", new AttributeValue { N = "42" } }
    };
    
    private Dictionary<string, AttributeValue> newImage = new()
    {
        { "PartitionKey", new AttributeValue { S = "VISITOR" } },
        { "Id", new AttributeValue { S = "FOO" } },
        { "TotalViews", new AttributeValue { N = "43" } },
        { "Expiration", new AttributeValue { N = "42" } }
    };
    
    private readonly string month = "202305";
        
    public ViewsAggregatorTests()
    {
        dbMock.Setup(x => x.FromDocument<Visitor>(Document.FromAttributeMap(oldImage)))
            .Returns(new Visitor("FOO", 42) { TotalViews = 42 });

        dbMock.Setup(x => x.FromDocument<Visitor>(Document.FromAttributeMap(newImage)))
            .Returns(new Visitor("FOO", 42) { TotalViews = 43 });

        dateTimeProviderMock.Setup(x => x.GetCurrentYearAndMonthDatePartString())
            .Returns(month);

        if (AWSConfigsDynamoDB.Context.TypeMappings.ContainsKey(typeof(ViewStatistics)))
        {
            AWSConfigsDynamoDB.Context.TypeMappings.Remove(typeof(ViewStatistics));
        }
        
        if (AWSConfigsDynamoDB.Context.TypeMappings.ContainsKey(typeof(Visitor)))
        {
            AWSConfigsDynamoDB.Context.TypeMappings.Remove(typeof(Visitor));
        }
    }
    
    [Fact]
    public async Task HandleIncrementsUniqueVisitorsOnInsert()
    {
        var actual = new ViewStatistics(month);
        
        dbMock.Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", month, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(actual);
        
        dbMock.Setup(x => x.SaveAsync(actual, It.IsAny<CancellationToken>()))
            .Verifiable();
        
        var @event = CreateEvent(OperationType.INSERT);

        var sut = new ViewsAggregator(dbMock.Object, dateTimeProviderMock.Object);

        await sut.Handle(@event, new TestLambdaContext());
        
        Assert.Equal(1, actual.TotalViews);
        Assert.Equal(1, actual.UniqueVisitors);
        dbMock.VerifyAll();
    }
    
    [Fact]
    public async Task HandleIncrementsUniqueVisitorsOnModify()
    {
        var actual = new ViewStatistics(month)
        {
            TotalViews = 1,
            UniqueVisitors = 1
        };
        
        dbMock.Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", month, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(actual);
        
        dbMock.Setup(x => x.SaveAsync(actual, It.IsAny<CancellationToken>()))
            .Verifiable();
        
        var @event = CreateEvent(OperationType.MODIFY);

        var sut = new ViewsAggregator(dbMock.Object, dateTimeProviderMock.Object);

        await sut.Handle(@event, new TestLambdaContext());
        
        Assert.Equal(2, actual.TotalViews);
        Assert.Equal(1, actual.UniqueVisitors);
        dbMock.VerifyAll();
    }

    private DynamoDBEvent CreateEvent(OperationType operationType)
    {
        return new DynamoDBEvent
        {
            Records = new List<DynamoDBEvent.DynamodbStreamRecord>
            {
                new()
                {
                    EventName = operationType,
                    Dynamodb = new StreamRecord
                    {
                        NewImage = newImage,
                        OldImage = oldImage,
                        StreamViewType = StreamViewType.NEW_AND_OLD_IMAGES
                    }
                }
            }
        };
    }
}