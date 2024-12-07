namespace ConsulExtension.Models;

public sealed class ServiceConfig
{
    public ServiceConfig(Action<ServiceConfig> configOverride) =>configOverride.Invoke(this);
    
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int Http1Port { get; set; }
    public int GrpcPort { get; set; }
    public string HealthCheckEndpoint { get; set; } = null!;
}