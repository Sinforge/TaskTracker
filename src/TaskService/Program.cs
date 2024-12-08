using System.Text;
using AuthService.Settings;
using ConsulExtension;
using ConsulExtension.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskService.Api.Http.Grpc;
using TaskService.Data;
using TaskService.Settings;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Microsoft.AspNetCore.Server.Kestrel.EnableWindows81Http2", true);
builder.Services    
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddConsulClient(builder.Configuration);
    
var appSettings = new AppSettings();
builder.Services.AddGrpc();
builder.Configuration.AddConsulConfiguration(["TaskService", "JwtSecretKey"]).Build().Bind(appSettings);
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
    .AddDbContext<TaskServiceDbContext>(options =>
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
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskServiceDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcReflectionService();
app.MapGrpcService<CustomHealthCheckService>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

