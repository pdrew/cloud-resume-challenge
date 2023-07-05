using Amazon.DynamoDBv2.DataModel;
using BackEnd.Api.Controllers;
using BackEnd.Api.Services;
using BackEnd.Shared.Models;
using BackEnd.Shared.Services;
using Moq;
using Xunit;

namespace BackEnd.Tests;

public class ViewsControllerTests
{
    private Mock<IDynamoDBContext> dbMock = new();
    private Mock<IDateTimeProvider> dateTimeProviderMock = new();
    private Mock<AsyncSearch<Visitor>> asyncSearchMock = new();

    [Fact]
    public async Task IndexReturnsCorrectResult()
    {
        var month = "202305";
        
        dateTimeProviderMock.Setup(x => x.GetCurrentYearAndMonthDatePartString())
            .Returns(month);
        
        dateTimeProviderMock.Setup(x => x.TimestampExpired(It.IsAny<long?>())).Returns(true);
        
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ViewStatistics(month) { TotalViews = 42 });
    
        var sut = new ViewsController(dbMock.Object, dateTimeProviderMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(42, actual.TotalViews);
    }
    
    [Fact]
    public async Task IndexReturnsCorrectResultWhenTableEmpty()
    {
        var month = "202305";
        
        dateTimeProviderMock.Setup(x => x.GetCurrentYearAndMonthDatePartString())
            .Returns(month);

        dateTimeProviderMock.Setup(x => x.TimestampExpired(It.IsAny<long?>())).Returns(true);
        
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", month, It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ViewStatistics)null!);

        var sut = new ViewsController(dbMock.Object, dateTimeProviderMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(0, actual.TotalViews);
        Assert.Equal(month, actual.Month);
    }
    
    [Fact]
    public async Task IndexQueriesVisitorsWhenTimestampHasNotExpired()
    {
        var month = "202305";
        
        dateTimeProviderMock.Setup(x => x.GetCurrentYearAndMonthDatePartString())
            .Returns(month);

        dateTimeProviderMock.Setup(x => x.TimestampExpired(It.IsAny<long?>())).Returns(false);

        asyncSearchMock.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Visitor>()
            {
                new () { TotalViews = 1 },
                new () { TotalViews = 1 },
                new () { TotalViews = 2 },
            });
        
        dbMock
            .Setup(x => x.QueryAsync<Visitor>("VISITOR", It.IsAny<DynamoDBOperationConfig>()))!
            .Returns(asyncSearchMock.Object);
        
        var sut = new ViewsController(dbMock.Object, dateTimeProviderMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(3, actual.UniqueVisitors);
        Assert.Equal(4, actual.TotalViews);
        Assert.Equal(month, actual.Month);
    }
}