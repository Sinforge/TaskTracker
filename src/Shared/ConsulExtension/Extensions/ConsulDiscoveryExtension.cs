using Consul;
using ConsulExtension.Models;
using ConsulExtension.Services.Implementations;
using ConsulExtension.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsulExtension;

public static class ConsulDiscoveryExtension
{
    public static IServiceCollection AddConsulClient(this IServiceCollection services, IConfiguration configuration)
    {
        var consulClient = new ConsulClient(config => { config.Address = configuration.GetValue<Uri>("ConsulUrl")!; });
        return services.AddTransient<IConsulClient, ConsulClient>(_ => consulClient)
            .AddScoped<IConsulServiceDiscovery, ConsulServiceDiscovery>();
    }

    public static IServiceCollection AddConsulDiscovery(this IServiceCollection services,
        Action<ServiceConfig> serviceConfigOverride)
    {
        var serviceConfig = new ServiceConfig(serviceConfigOverride);
        return services
            .AddSingleton(serviceConfig)
            .AddSingleton<IHostedService, ConsulDiscoveryHostedService>();
    }
}