using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using BackEnd.Controllers;
using BackEnd.Models;
using Moq;
using Xunit;

namespace BackEnd.Tests;

public class ViewsControllerTests
{
    private Mock<IDynamoDBContext> dbMock = new ();

    [Fact]
    public async Task IndexReturnsCorrectResult()
    {
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("ViewStatistics", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ViewStatistics() { Total = 42 });

        var sut = new ViewsController(dbMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(42, actual.Total);
    }
    
    [Fact]
    public async Task IndexReturnsCorrectResultWhenTableEmpty()
    {
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("ViewStatistics", It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ViewStatistics)null);

        var sut = new ViewsController(dbMock.Object);

        var actual = await sut.Index();
        
        Assert.Equal(0, actual.Total);
    }
    
    [Fact]
    public async Task IncrementReturnsCorrectResult()
    {
        var viewStatistics = new ViewStatistics() { Total = 42 };
        
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("ViewStatistics", It.IsAny<CancellationToken>()))
            .ReturnsAsync(viewStatistics);

        dbMock
            .Setup(x => x.SaveAsync(viewStatistics, It.IsAny<CancellationToken>()))
            .Verifiable();

        var sut = new ViewsController(dbMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(43, actual.Total);
        dbMock.VerifyAll();
    }
    
    [Fact]
    public async Task IncrementReturnsCorrectResultWhenTableEmpty()
    {
        dbMock
            .Setup(x => x.LoadAsync<ViewStatistics>("ViewStatistics", It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ViewStatistics)null);

        dbMock
            .Setup(x => x.SaveAsync(It.IsAny<ViewStatistics>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        var sut = new ViewsController(dbMock.Object);

        var actual = await sut.Increment();
        
        Assert.Equal(1, actual.Total);
        dbMock.VerifyAll();
    }
    
    
}