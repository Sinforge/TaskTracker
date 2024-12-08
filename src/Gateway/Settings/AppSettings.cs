namespace Gateway.Settings;

public class AppSettings
{
    public string ConsulUrl { get; init; } = null!;
    public AudienceSettings Audience { get; init; } = null!;
    public GitHubSettings GitHub { get; init; } = null!;
    public ConnectionStrings ConnectionStrings { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string HealthCheckEndpoint { get; init; } = null!;
    public InstanceSettings InstanceConfig { get; init; } = null!;
}