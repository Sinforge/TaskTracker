using AuthService.Data;
using AuthService.Requests;
using AuthService.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[Route("api/[controller]")]
public class UsersController(AuthServiceDbContext dbContext, IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var userWithSameLogin = await dbContext.Users.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken: cancellationToken);
        if (userWithSameLogin is not null)
            return BadRequest("User with such login already exists");

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            Name = request.Name,
            PasswordHash = PasswordUtils.HashPassword(request.Password),
            Surname = request.Surname
        };
        
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var token = JwtTokenUtils.GenerateToken(user.Id, GetSecretKey());
        return Ok(new { access_token = token });
    }

    [NonAction]
    private string GetSecretKey() => configuration.GetValue<string>("SecretKey")!; 

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken);
        if (user is null)
            return BadRequest();

        var hasCorrectPassword = PasswordUtils.VerifyPassword(request.Password, user.PasswordHash);
        if (!hasCorrectPassword)
            return BadRequest();
        
        var token = JwtTokenUtils.GenerateToken(user.Id, GetSecretKey());
        return Ok(new { access_token = token });
    }
    
    
}