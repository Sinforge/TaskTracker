using System.Text;
using ConsulExtension.Extensions;
using Gateway;
using Gateway.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddConsul<MyConsulServiceBuilder>();

var appSettings = new AppSettings();
builder.Services.AddConsulClient(builder.Configuration);
builder.Configuration.AddConsulConfiguration(["JwtSecretKey"]).Build().Bind(appSettings);

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
var app = builder.Build();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseOcelot();
app.Run();