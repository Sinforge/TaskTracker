using ConsulExtension.Models;
using Microsoft.Extensions.Configuration;

namespace ConsulExtension;

public static class ConfigurationExtension
{
    public static ServiceConfig GetServiceConfig(this IConfiguration configuration)
    {
        var serviceSection = configuration.GetSection("ServiceConfig")!;

        return new ServiceConfig
            {
                Id = serviceSection.GetValue<string>("Id")!,
                Name = serviceSection.GetValue<string>("Name")!,
                Url = serviceSection.GetValue<string>("Url")!,
                Port = serviceSection.GetValue<int>("Port"),
                ConsulUrl = serviceSection.GetValue<Uri>("ConsulUrl")!,
                HealthCheckEndpoint = serviceSection.GetValue<string>("HealthCheckEndpoint")!
            };
    }
}