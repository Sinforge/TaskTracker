using Consul;
using ConsulExtension.Services.Interfaces;

namespace ConsulExtension.Services.Implementations;

public class ConsulServiceDiscovery(IConsulClient consulClient) : IConsulServiceDiscovery
{
    public async Task<string> GetServiceUrlAsync(string serviceName, string tag)
    {
        var services = await consulClient.Catalog.Service(serviceName);
        var service = services.Response.FirstOrDefault(x => x.ServiceTags.Contains(tag));

        if (service == null)
            throw new Exception($"Service {serviceName} not found in Consul.");

        return $"http://{service.ServiceAddress}:{service.ServicePort}";
    }
}