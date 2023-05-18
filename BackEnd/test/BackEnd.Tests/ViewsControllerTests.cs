using Amazon.DynamoDBv2.DataModel;
using BackEnd.Controllers;
using BackEnd.Models;
using BackEnd.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace BackEnd.Tests;

public class ViewsControllerTests
{
    private Mock<IDynamoDBContext> dbMock = new ();

    private Mock<IClientIpAccessor> clientIpAccessorMock = new();

    [Fact]
    public async Task IndexReturnsCorrectResult()
    {
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", "VIEWS", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ViewStatistics() { TotalViews = 42 });

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns("127.0.0.1");
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(42, actual.TotalViews);
    }
    
    [Fact]
    public async Task IndexReturnsCorrectResultWhenTableEmpty()
    {
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", "VIEWS", It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ViewStatistics)null!);

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns("127.0.0.1");
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(0, actual.TotalViews);
    }
    
    /*
    [Fact]
    public async Task IncrementReturnsCorrectResult()
    {
        var viewStatistics = new ViewStatistics() { TotalViews = 42 };
        
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", "VIEWS", It.IsAny<CancellationToken>()))
            .ReturnsAsync(viewStatistics);

        dbMock
            .Setup(x => x.SaveAsync(viewStatistics, It.IsAny<CancellationToken>()))
            .Verifiable();

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns("127.0.0.1");
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(43, actual.TotalViews);
        dbMock.VerifyAll();
    }
    
    [Fact]
    public async Task IncrementReturnsCorrectResultWhenTableEmpty()
    {
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("STATISTICS", "VIEWS", It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ViewStatistics)null!);

        dbMock
            .Setup(x => x.SaveAsync(It.IsAny<ViewStatistics>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns("127.0.0.1");
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(1, actual.TotalViews);
        dbMock.VerifyAll();
    }
    */
}