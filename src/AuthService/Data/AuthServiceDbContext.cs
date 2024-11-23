using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class AuthServiceDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    
    public AuthServiceDbContext() {}

    public AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options) : base(options) { }


}