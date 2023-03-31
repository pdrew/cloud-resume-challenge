using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[ApiController]
[Route("[controller]")]
public class VisitsController : ControllerBase
{
    // GET
    [HttpGet]
    public int Index()
    {
        return new Random().Next(1, 100);
    }
}