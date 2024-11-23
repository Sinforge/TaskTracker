using System.Text.Json.Serialization;

namespace TaskService.Api.Http.Contracts.AddTask;

public sealed record AddTaskResponse(
    [property: JsonPropertyName("id")] Guid Id);