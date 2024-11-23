using System.Text.Json.Serialization;

namespace AuthService.Requests;

public sealed record LoginRequest(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("password")] string Password
    );