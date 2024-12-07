using AuthService.Settings;
using ConsulExtension;
using ConsulExtension.Extensions;
using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Grpc;
using NotificationService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services    
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddConsulClient(builder.Configuration);
var appSettings = new AppSettings();
builder.Configuration.AddConsulConfiguration(["NotificationService", "JwtSecretKey"]).Build().Bind(appSettings);
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
    .AddDbContext<NotificationServiceDbContext>(options =>
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
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationServiceDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger(); 
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapGrpcService<NotificationService.Api.Grpc.NotificationService>();
app.MapGrpcReflectionService();
app.MapGrpcService<CustomHealthCheckService>();
app.MapControllers();
app.Run();