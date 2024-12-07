using Microsoft.Extensions.Configuration;
using Winton.Extensions.Configuration.Consul;

namespace ConsulExtension.Extensions;

public static class ConfigurationExtension
{
    public static IConfigurationManager AddConsulConfiguration(this IConfigurationManager configuration, string[] sections)
    {
        foreach (var section in sections)
        {
            configuration.AddConsul(section, o =>
                o.ConsulConfigurationOptions = c =>
                {
                    c.Address = new Uri(configuration.GetValue<string>("ConsulUrl")!);
                }
            );
        }
        
        return configuration;
    }
}