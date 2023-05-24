﻿namespace BackEnd.Shared.Services;

public interface IDateTimeProvider
{
    DateTimeOffset GetEndOfCurrentMonthUtc();

    string GetCurrentYearAndMonthDatePartString();
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetEndOfCurrentMonthUtc()
    {
        return GetEndOfMonthUtc(DateTimeOffset.UtcNow);
    }

    public string GetCurrentYearAndMonthDatePartString()
    {
        return GetYearAndMonthDatePartString(DateTimeOffset.UtcNow);
    }

    public DateTimeOffset GetEndOfMonthUtc(DateTimeOffset dateTimeOffset)
    {
        var year = dateTimeOffset.Year;
        var month = dateTimeOffset.Month;
        var days = DateTime.DaysInMonth(dateTimeOffset.Year, dateTimeOffset.Month);

        return new DateTimeOffset(year, month, days, 23, 59, 59, dateTimeOffset.Offset).ToUniversalTime();
    }
    
    public string GetYearAndMonthDatePartString(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("yyyyMM");
    }
}