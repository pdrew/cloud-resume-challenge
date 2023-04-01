using BackEnd.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class ViewsController : ControllerBase
{
    // GET
    [HttpGet]
    public ViewStatistics Index()
    {
        var total = new Random().Next(1, 100);

        return new ViewStatistics()
        {
            Total = total
        };
    }
}