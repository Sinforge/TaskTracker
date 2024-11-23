using System.Text.Json.Serialization;

namespace TaskService.Api.Http.Contracts.GetAllTasks;

public sealed record GetAllTasksDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("creatingTimestampUtc")] DateTime CreatingTimestampUtc,
    [property: JsonPropertyName("creatorId")] Guid CreatorId
    );