namespace ConsulExtension.Models;

public sealed class ServiceConfig
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required int Port {get; init; }
    public required Uri ConsulUrl { get; init; }
    public required string HealthCheckEndpoint { get; init; }
}