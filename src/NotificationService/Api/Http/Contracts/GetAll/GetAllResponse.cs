using System.Text.Json.Serialization;

namespace NotificationService.Api.Http.Contracts.GetAll;

public sealed record GetAllResponse(
    [property: JsonPropertyName("notifications")] IEnumerable<GetAllDto> Notifications);