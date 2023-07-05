using Amazon.DynamoDBv2.DataModel;
using BackEnd.Api.Services;
using BackEnd.Shared.Models;
using BackEnd.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ViewsController : ControllerBase
{
    private readonly IDynamoDBContext db;
    private readonly IDateTimeProvider dateTimeProvider;

    public ViewsController(IDynamoDBContext db, IDateTimeProvider dateTimeProvider)
    {
        this.db = db;
        this.dateTimeProvider = dateTimeProvider;
    }
    
    // GET
    [HttpGet]
    public async Task<ViewStatistics> Index(long? timestamp = null)
    {
        var month = dateTimeProvider.GetCurrentYearAndMonthDatePartString();

        if (dateTimeProvider.TimestampExpired(timestamp))
        {
            return await db.LoadAsync<ViewStatistics>("STATISTICS", month) ?? new ViewStatistics(month);
        }

        var visitors = await db.QueryAsync<Visitor>("VISITOR").GetRemainingAsync();

        return new ViewStatistics(month)
        {
            UniqueVisitors = visitors?.Count ?? 0,
            TotalViews = visitors?.Sum(x => x.TotalViews) ?? 0
        };
    }
}