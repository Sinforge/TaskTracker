using System.Text.Json.Serialization;

namespace AuthService.Requests;

public sealed record RegisterUserRequest(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("surname")] string Surname,
    [property: JsonPropertyName("password")] string Password
    );