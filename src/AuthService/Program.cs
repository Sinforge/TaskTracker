using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Data;
using AuthService.Grpc.Services;
using AuthService.Settings;
using ConsulExtension.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services    
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddConsulClient(builder.Configuration);

var appSettings = new AppSettings();
builder.Configuration.AddConsulConfiguration(["AuthService", "JwtSecretKey", "GitHubApiSecrets"]).Build().Bind(appSettings);
builder.Services.Configure<AppSettings>(builder.Configuration)
    .AddCors(options =>
        {
            options.AddPolicy("AllowGitHub", policy =>
            {
                policy.WithOrigins("https://github.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        }
    )
    .AddConsulDiscovery(config =>
    {
        config.Id = appSettings.InstanceConfig.Id.ToString();
        config.Url = appSettings.InstanceConfig.Url;
        config.Http1Port = appSettings.InstanceConfig.Http1Port;
        config.GrpcPort = appSettings.InstanceConfig.GrpcPort;
        config.Name = appSettings.Name;
        config.HealthCheckEndpoint = appSettings.HealthCheckEndpoint;
    })
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = "Cookies";  // Используем схему Cookie для SignIn
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";  // Путь для входа
        options.LogoutPath = "/logout";  // Путь для выхода
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
    })
    .AddGitHub(options =>
    {
        options.ClientId = appSettings.GitHub.ClientId;
        options.ClientSecret = appSettings.GitHub.ClientSecret;
        options.Scope.Add("read:user");
        options.SaveTokens = true;
        options.CallbackPath = "/api/github-oauth";
        options.Events.OnCreatingTicket = async context =>
        {

            var db = context.HttpContext.RequestServices.GetRequiredService<AuthServiceDbContext>();
            var githubId = int.Parse(context.Identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var name = context.Identity?.FindFirst(ClaimTypes.Name)?.Value;

            var user = db.Users.FirstOrDefault(u => u.GitHubId == githubId);
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    GitHubId = githubId,
                    Name = name,
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            var claims = new[]
            {
                new Claim("GitHubId", user.GitHubId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Audience.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: appSettings.Audience.Iss,
                audience: appSettings.Audience.Aud,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var authToken =  new JwtSecurityTokenHandler().WriteToken(token);

            context.Response.Cookies.Append("JWT_Token", authToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(30) 
            });

            context.Response.Headers.Append("Authorization", "Bearer " + authToken);
        };
    }).Services
    .AddDbContext<AuthServiceDbContext>(options =>
    {
        var connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
        options.UseNpgsql(connection);
    })
    .AddControllers();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseCors("AllowGitHub");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthServiceDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.MapGrpcService<GrpcUserService>();
app.MapGrpcReflectionService();
app.MapGrpcService<CustomHealthCheckService>();

app.MapGet("api/login/github", async context =>
{
    await context.ChallengeAsync("GitHub", new AuthenticationProperties
    {
        RedirectUri = "http://localhost:8000/login/success"
    });
});

app.Run();
