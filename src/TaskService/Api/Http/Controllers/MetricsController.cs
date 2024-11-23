using Microsoft.AspNetCore.Mvc;

namespace TaskService.Api.Http.Controllers;

[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth() => Ok();
}