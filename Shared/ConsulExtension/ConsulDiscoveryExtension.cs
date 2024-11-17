using Consul;
using ConsulExtension.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsulExtension;

public static class ConsulDiscoveryExtension
{
    public static IServiceCollection AddConsulDiscovery(this IServiceCollection services, ServiceConfig serviceConfig)
    {
        var consulClient = new ConsulClient(config =>
        {
            config.Address = serviceConfig.ConsulUrl;
        });

        return services.AddSingleton(serviceConfig)
            .AddSingleton<IConsulClient, ConsulClient>(_ => consulClient)
            .AddSingleton<IHostedService, ConsulDiscoveryHostedService>();
    }
}