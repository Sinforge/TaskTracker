namespace ConsulExtension.Services.Interfaces;

public interface IConsulServiceDiscovery
{
    Task<string> GetServiceUrlAsync(string serviceName, string tag);
}