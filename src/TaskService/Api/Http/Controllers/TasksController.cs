using ConsulExtension.Services.Interfaces;
using Grpc.Net.Client;
using GrpcNotificationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskService.Api.Http.Contracts.AddTask;
using TaskService.Api.Http.Contracts.GetAllTasks;
using TaskService.Api.Http.Contracts.UpdateTask;
using TaskService.Data;
using Task = TaskService.Data.Task;

namespace TaskService.Api.Http.Controllers;

[Route("api/[controller]")]
public class TasksController(TaskServiceDbContext dbContext, IConsulServiceDiscovery serviceDiscovery) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] AddTaskRequest request, CancellationToken cancellationToken)
    {
        var newTask = new Task
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatingTimestampUtc = DateTime.UtcNow,
            CreatorId = Guid.NewGuid()
        };
        await dbContext.Tasks.AddAsync(newTask, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var serviceUrl = await serviceDiscovery.GetServiceUrlAsync("NotificationService", "grpc");
        var channel = GrpcChannel.ForAddress(serviceUrl);
        Console.WriteLine(serviceUrl);
        var client = new NotificationService.NotificationServiceClient(channel);
        await client.SendNotificationAsync(new()
        {
            Message = $"Task with Id {newTask.Id} created",
            UserId = newTask.CreatorId.ToString()
        });
        
        return Ok(new AddTaskResponse(newTask.Id));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => request.Id == x.Id, cancellationToken);
        if (task is null)
            return NotFound("task not found");

        task.Description = request.Description;
        task.Name = request.Name;

        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        var serviceUrl = await serviceDiscovery.GetServiceUrlAsync("NotificationService", "grpc");
        var channel = GrpcChannel.ForAddress(serviceUrl);
        var client = new NotificationService.NotificationServiceClient(channel);
        await client.SendNotificationAsync(new()
        {
            Message = $"Task with Id {task.Id} updated",
            UserId = task.CreatorId.ToString()
        });
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (task is null)
            return NotFound();

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        var serviceUrl = await serviceDiscovery.GetServiceUrlAsync("NotificationService", "grpc");
        var channel = GrpcChannel.ForAddress(serviceUrl);
        var client = new NotificationService.NotificationServiceClient(channel);
        await client.SendNotificationAsync(new()
        {
            Message = $"Task with Id {id} created",
            UserId = id.ToString()
        });

        return Ok();
    }

    [HttpGet]
    public IActionResult GetAllAsync()
    {
        var tasks = dbContext.Tasks.Select(x =>
            new GetAllTasksDto(x.Id, x.Name, x.Description, x.CreatingTimestampUtc, x.CreatorId)).ToList();
        return Ok(new GetAllTasksResponse(tasks));
    }
}