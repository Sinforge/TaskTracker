using Consul;
using ConsulExtension.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsulExtension;

public class ConsulDiscoveryHostedService(
    ILogger<ConsulDiscoveryHostedService> logger,
    IConsulClient client,
    ServiceConfig config
    ) : IHostedService
{
    private AgentServiceRegistration _serviceRegistration = null!;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _serviceRegistration = new AgentServiceRegistration()
        {
            ID = config.Id,
            Name = config.Name,
            Address = config.Url,
            Port = config.Port,
            Check = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(15),
                HTTP = $"http://{config.Url}:{config.Port}/api/values/{config.HealthCheckEndpoint}",
                Timeout = TimeSpan.FromSeconds(5)
            }
        };

        try
        {
            await client.Agent.ServiceDeregister(_serviceRegistration.ID, cancellationToken).ConfigureAwait(false);
            await client.Agent.ServiceRegister(_serviceRegistration, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            logger.LogError($"Error while trying to deregister in {nameof(StartAsync)}");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await client.Agent.ServiceDeregister(_serviceRegistration.ID, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            logger.LogError($"Error while trying to deregister in {nameof(StopAsync)}");
        }
    }
}