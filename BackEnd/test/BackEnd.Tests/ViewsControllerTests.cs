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
    
    [Fact]
    public async Task IncrementReturnsCorrectResult()
    {
        var clientIp = "127.0.0.1";

        var hash = new HashingService().HashString(clientIp);
        
        var visitor = new Visitor(hash) { Views = 42 };
        
        dbMock
            .Setup(x => x.LoadAsync<Visitor>("VISITOR", hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visitor);

        dbMock
            .Setup(x => x.SaveAsync(visitor, It.IsAny<CancellationToken>()))
            .Verifiable();

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns(clientIp);
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(43, actual.Views);
        dbMock.VerifyAll();
    }
    
    [Fact]
    public async Task IncrementReturnsCorrectResultWhenTableEmpty()
    {
        var clientIp = "127.0.0.1";

        var hash = new HashingService().HashString(clientIp);
        
        dbMock
            .Setup(x => x.LoadAsync<Visitor>("VISITOR", hash, It.IsAny<CancellationToken>()))!
            .ReturnsAsync((Visitor)null!);

        dbMock
            .Setup(x => x.SaveAsync(It.IsAny<Visitor>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        clientIpAccessorMock.Setup(x => x.GetClientIp()).Returns("127.0.0.1");
    
        var sut = new ViewsController(dbMock.Object, clientIpAccessorMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(1, actual.Views);
        dbMock.VerifyAll();
    }
}