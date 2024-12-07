using Consul;
using ConsulExtension.Models;
using ConsulExtension.Services;
using ConsulExtension.Services.Implementations;
using ConsulExtension.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsulExtension.Extensions;

public static class ConsulDiscoveryExtension
{
    public static IServiceCollection AddConsulClient(this IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine( configuration.GetValue<Uri>("ConsulUrl")!);
        var consulClient = new ConsulClient(config => { config.Address = configuration.GetValue<Uri>("ConsulUrl")!; });
        return services.AddSingleton<IConsulClient, ConsulClient>(_ => consulClient)
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