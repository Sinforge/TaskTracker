using Consul;
using Ocelot.Logging;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Consul.Interfaces;

namespace Gateway;

public class MyConsulServiceBuilder(IHttpContextAccessor contextAccessor, IConsulClientFactory clientFactory, IOcelotLoggerFactory loggerFactory)
    : DefaultConsulServiceBuilder(contextAccessor, clientFactory, loggerFactory)
{
    public override bool IsValid(ServiceEntry entry) => entry.Service.Tags.Contains("http");

    // I want to use the agent service IP address as the downstream hostname
    protected override string GetDownstreamHost(ServiceEntry entry, Node node) => entry.Service.Address;
}