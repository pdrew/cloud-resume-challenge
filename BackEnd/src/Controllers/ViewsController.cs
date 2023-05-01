using Amazon.DynamoDBv2.DataModel;
using BackEnd.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class ViewsController : ControllerBase
{
    private readonly IDynamoDBContext db;

    public ViewsController(IDynamoDBContext db)
    {
        this.db = db;
    }
    
    // GET
    [HttpGet]
    public async Task<ViewStatistics> Index()
    {
        var statistics = await db.LoadAsync<ViewStatistics>(nameof(ViewStatistics)) ?? new ViewStatistics();

        return statistics;
    }

    [HttpPost]
    public async Task<ViewStatistics> Increment()
    {
        var statistics = await db.LoadAsync<ViewStatistics>(nameof(ViewStatistics)) ?? new ViewStatistics();

        statistics.Total++;

        await db.SaveAsync(statistics);

        return statistics;
    }
}