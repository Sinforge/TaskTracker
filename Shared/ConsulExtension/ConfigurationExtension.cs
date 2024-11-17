using ConsulExtension.Models;
using Microsoft.Extensions.Configuration;

namespace ConsulExtension;

public static class ConfigurationExtension
{
    public static ServiceConfig GetServiceConfig(this IConfiguration configuration)
    {
        return configuration.GetValue<ServiceConfig>("ServiceConfig")!;
    }
}