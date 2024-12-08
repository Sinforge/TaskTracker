using System.Text;
using ConsulExtension.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Api.Grpc;
using NotificationService.Data;
using NotificationService.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services    
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddConsulClient(builder.Configuration);
var appSettings = new AppSettings();
builder.Configuration.AddConsulConfiguration(["NotificationService", "JwtSecretKey"]).Build().Bind(appSettings);
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Audience.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = appSettings.Audience.Iss,
            ValidateAudience = true,
            ValidAudience = appSettings.Audience.Aud,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
        };
    });
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

app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<NotificationService.Api.Grpc.NotificationService>();
app.MapGrpcReflectionService();
app.MapGrpcService<CustomHealthCheckService>();
app.MapControllers();
app.Run();