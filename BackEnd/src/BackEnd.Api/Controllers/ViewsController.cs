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
    private readonly IClientIpAccessor clientIpAccessor;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IHashingService hashingService;

    public ViewsController(IDynamoDBContext db, IClientIpAccessor clientIpAccessor, 
        IDateTimeProvider dateTimeProvider, IHashingService hashingService)
    {
        this.db = db;
        this.clientIpAccessor = clientIpAccessor;
        this.dateTimeProvider = dateTimeProvider;
        this.hashingService = hashingService;
    }
    
    // GET
    [HttpGet]
    public async Task<ViewStatistics> Index()
    {
        var month = dateTimeProvider.GetCurrentYearAndMonthDatePartString();
        
        var statistics = await db.LoadAsync<ViewStatistics>("STATISTICS", month) ?? new ViewStatistics(month);

        return statistics;
    }

    [HttpPost]
    public async Task<Visitor> Increment()
    {
        var clientIp = clientIpAccessor.GetClientIp();

        var expiration = dateTimeProvider.GetEndOfCurrentMonthUtc().ToUnixTimeSeconds();
        
        var hash = hashingService.HashString(clientIp, expiration.ToString());
        
        var visitor = await db.LoadAsync<Visitor>("VISITOR", hash) ?? new Visitor(hash, expiration);
        
        visitor.TotalViews++;

        await db.SaveAsync(visitor);

        return visitor;
    }
}