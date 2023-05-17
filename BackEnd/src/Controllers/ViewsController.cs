using Amazon.DynamoDBv2.DataModel;
using BackEnd.Models;
using BackEnd.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class ViewsController : ControllerBase
{
    private readonly IDynamoDBContext db;
    private readonly IClientIpAccessor clientIpAccessor;

    public ViewsController(IDynamoDBContext db, IClientIpAccessor clientIpAccessor)
    {
        this.db = db;
        this.clientIpAccessor = clientIpAccessor;
    }
    
    // GET
    [HttpGet]
    public async Task<ViewStatistics> Index()
    {
        var statistics = await db.LoadAsync<ViewStatistics>("STATISTICS", "VIEWS") ?? new ViewStatistics();

        return statistics;
    }

    [HttpPost]
    public async Task<ViewStatistics> Increment()
    {
        var clientIp = clientIpAccessor.GetClientIp();

        var hash = new HashingService().HashString(clientIp);

        var statistics = await db.LoadAsync<ViewStatistics>("STATISTICS", "VIEWS") ?? new ViewStatistics();
        
        var visitor = await db.LoadAsync<Visitor>("VISITOR", hash) ?? new Visitor(hash);

        if (visitor.IsNew())
        {
            statistics.UniqueVisitors++;
        }
        
        visitor.Views++;
        statistics.TotalViews++;

        await db.SaveAsync(visitor);
        await db.SaveAsync(statistics);

        return statistics;
    }
}