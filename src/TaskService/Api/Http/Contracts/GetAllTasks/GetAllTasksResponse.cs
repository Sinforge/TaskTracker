using System.Text.Json.Serialization;

namespace TaskService.Api.Http.Contracts.GetAllTasks;

public sealed record GetAllTasksResponse(
    [property: JsonPropertyName("tasks")] IReadOnlyCollection<GetAllTasksDto> Tasks);