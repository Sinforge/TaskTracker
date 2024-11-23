using Microsoft.Extensions.Configuration;
using Winton.Extensions.Configuration.Consul;

namespace ConsulExtension;

public static class ConfigurationExtension
{
    public static IConfigurationManager AddConsulConfiguration(this IConfigurationManager configuration, string[] sections)
    {
        foreach (var section in sections)
        {
            configuration.AddConsul(section);
        }
        
        return configuration;
    }
}