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
    private Mock<IClientIpAccessor> clientIpAccessorMock = new();
    private Mock<IDateTimeProvider> dateTimeProviderMock = new();
    private Mock<IHashingService> hashingServiceMock = new();
    
    [Fact]
    public async Task IndexReturnsCorrectResult()
    {
        var month = "202305";
        
        dateTimeProviderMock.Setup(x => x.GetCurrentYearAndMonthDatePartString())
            .Returns(month);
        
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ViewStatistics(month) { TotalViews = 42 });
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object, dateTimeProviderMock.Object, hashingServiceMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(42, actual.TotalViews);
    }
    
    [Fact]
    public async Task IndexReturnsCorrectResultWhenTableEmpty()
    {
        var month = "202305";
        
        dateTimeProviderMock.Setup(x => x.GetCurrentYearAndMonthDatePartString())
            .Returns(month);
        
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", month, It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ViewStatistics)null!);

        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object, dateTimeProviderMock.Object, hashingServiceMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(0, actual.TotalViews);
        Assert.Equal(month, actual.Month);
    }
    
    [Fact]
    public async Task IncrementReturnsCorrectResult()
    {
        var clientIp = "127.0.0.1";
        var hash = "FOO";
        var endOfMonth = new DateTimeOffset(2023, 5, 31, 23, 59, 59, DateTimeOffset.UtcNow.Offset);
        
        var visitor = new Visitor(hash, 42) { TotalViews = 42 };

        dateTimeProviderMock.Setup(x => x.GetEndOfCurrentMonthUtc())
            .Returns(endOfMonth);

        hashingServiceMock.Setup(x => x.HashString(clientIp, endOfMonth.ToUnixTimeSeconds().ToString()))
            .Returns(hash);
        
        dbMock
            .Setup(x => x.LoadAsync<Visitor>("VISITOR", hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visitor);

        dbMock
            .Setup(x => x.SaveAsync(visitor, It.IsAny<CancellationToken>()))
            .Verifiable();

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns(clientIp);
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object, dateTimeProviderMock.Object, hashingServiceMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(43, actual.TotalViews);
        dbMock.VerifyAll();
    }
    
    [Fact]
    public async Task IncrementReturnsCorrectResultWhenTableEmpty()
    {
        var clientIp = "127.0.0.1";
        var hash = "FOO";
        var endOfMonth = new DateTimeOffset(2023, 5, 31, 23, 59, 59, DateTimeOffset.UtcNow.Offset);
        
        dateTimeProviderMock.Setup(x => x.GetEndOfCurrentMonthUtc())
            .Returns(endOfMonth);

        hashingServiceMock.Setup(x => x.HashString(clientIp, endOfMonth.ToUnixTimeSeconds().ToString()))
            .Returns(hash);
        
        dbMock
            .Setup(x => x.LoadAsync<Visitor>("VISITOR", hash, It.IsAny<CancellationToken>()))!
            .ReturnsAsync((Visitor)null!);

        dbMock
            .Setup(x => x.SaveAsync(It.IsAny<Visitor>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns(clientIp);
 
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object, dateTimeProviderMock.Object, hashingServiceMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(1, actual.TotalViews);
        dbMock.VerifyAll();
    }
}