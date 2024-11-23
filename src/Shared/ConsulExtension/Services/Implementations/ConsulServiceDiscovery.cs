using Consul;
using ConsulExtension.Services.Interfaces;

namespace ConsulExtension.Services.Implementations;

public class ConsulServiceDiscovery(IConsulClient consulClient) : IConsulServiceDiscovery
{
    public async Task<string> GetServiceUrlAsync(string serviceName)
    {
        var services = await consulClient.Catalog.Service(serviceName);
        var service = services.Response.FirstOrDefault();

        if (service == null)
            throw new Exception($"Service {serviceName} not found in Consul.");

        return $"{service.ServiceAddress}:{service.ServicePort}";
    }
}