using System.Text.Json.Serialization;

namespace NotificationService.Api.Http.Contracts.GetAll;

public sealed record GetAllDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("userId")] Guid UserId);