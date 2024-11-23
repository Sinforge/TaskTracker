using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Http.Contracts.GetAll;
using NotificationService.Data;

namespace NotificationService.Api.Http.Controllers;

[Route("api/[controller]")]
public class NotificationsController(NotificationServiceDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllAsync() => Ok(dbContext.Notifications.Select(n => new GetAllDto(n.Id, n.Message, n.UserId)));
}