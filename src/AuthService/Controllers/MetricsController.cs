using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth() => Ok();
}