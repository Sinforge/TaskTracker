using Consul;
using ConsulExtension.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsulExtension.Services;

public class ConsulDiscoveryHostedService(
    ILogger<ConsulDiscoveryHostedService> logger,
    IConsulClient client,
    ServiceConfig config
    ) : IHostedService
{
    private AgentServiceRegistration _serviceRegistration = null!;
    private AgentServiceRegistration _grpcRegistration = null!;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _serviceRegistration = new AgentServiceRegistration()
        {
            ID = $"{config.Id}-http",
            Name = config.Name,
            Address = config.Url,
            Port = config.Http1Port,
            Tags = ["http"],
            Check = new AgentServiceCheck
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(15),
                HTTP = $"http://{config.Url}:{config.Http1Port}/{config.HealthCheckEndpoint}",
                Timeout = TimeSpan.FromSeconds(5)
            }
        };
        _grpcRegistration = new AgentServiceRegistration()
        {
            ID = $"{config.Id}-grpc",
            Name = config.Name,
            Address = config.Url,
            Port = config.GrpcPort,
            Tags = ["grpc"],
            Check = new AgentServiceCheck
            {
                GRPC = $"{config.Url}:{config.GrpcPort}",
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(15),
                Timeout = TimeSpan.FromSeconds(5)
            }
        };

        try
        {
            await client.Agent.ServiceDeregister(_serviceRegistration.ID, cancellationToken).ConfigureAwait(false);
            await client.Agent.ServiceRegister(_serviceRegistration, cancellationToken).ConfigureAwait(false);

            await client.Agent.ServiceDeregister(_grpcRegistration.ID, cancellationToken).ConfigureAwait(false);
            await client.Agent.ServiceRegister(_grpcRegistration, cancellationToken).ConfigureAwait(false);
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