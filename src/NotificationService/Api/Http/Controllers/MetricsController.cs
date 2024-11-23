using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Api.Http.Controllers;

[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth() => Ok();
}