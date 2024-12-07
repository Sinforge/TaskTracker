using AuthService.Data;
using AuthService.Grpc.Services;
using AuthService.Settings;
using ConsulExtension.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services    
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddConsulClient(builder.Configuration);
    
var appSettings = new AppSettings();
builder.Configuration.AddConsulConfiguration(["AuthService", "JwtSecretKey"]).Build().Bind(appSettings);
builder.Services.Configure<AppSettings>(builder.Configuration)
    .AddConsulDiscovery(config =>
    {
        config.Id = appSettings.InstanceConfig.Id.ToString();
        config.Url = appSettings.InstanceConfig.Url;
        config.Http1Port = appSettings.InstanceConfig.Http1Port;
        config.GrpcPort = appSettings.InstanceConfig.GrpcPort;
        config.Name = appSettings.Name;
        config.HealthCheckEndpoint = appSettings.HealthCheckEndpoint;
    })
    .AddDbContext<AuthServiceDbContext>(options =>
    {
        var connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
        options.UseNpgsql(connection);
    })
    .AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthServiceDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.MapGrpcService<GrpcUserService>();
app.MapGrpcReflectionService();
app.MapGrpcService<CustomHealthCheckService>();
app.Run();
