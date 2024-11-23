using System.Text.Json.Serialization;

namespace TaskService.Api.Http.Contracts.UpdateTask;

public sealed record UpdateTaskRequest(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description);