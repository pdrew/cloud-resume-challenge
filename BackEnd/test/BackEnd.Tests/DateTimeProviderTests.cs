using BackEnd.Shared.Services;
using Xunit;

namespace BackEnd.Tests;

public class DateTimeProviderTests
{
    [Fact]
    public void GetEndOfMonthUtcReturnsCorrectResult()
    {
        var dateTime = new DateTimeOffset(2023, 5, 24, 11, 11, 42, DateTimeOffset.UtcNow.Offset);

        var actual = new DateTimeProvider().GetEndOfMonthUtc(dateTime);
        
        var expected = new DateTimeOffset(2023, 5, 31, 23, 59, 59, DateTimeOffset.UtcNow.Offset);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetYearAndMonthDatePartStringReturnsCorrectResult()
    {
        var dateTime = new DateTimeOffset(2023, 5, 24, 11, 11, 42, DateTimeOffset.UtcNow.Offset);

        var actual = new DateTimeProvider().GetYearAndMonthDatePartString(dateTime);
        
        Assert.Equal("202305", actual);
    }
}