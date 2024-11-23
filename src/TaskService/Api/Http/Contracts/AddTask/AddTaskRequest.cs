using System.Text.Json.Serialization;

namespace TaskService.Api.Http.Contracts.AddTask;

public sealed record AddTaskRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description
    );