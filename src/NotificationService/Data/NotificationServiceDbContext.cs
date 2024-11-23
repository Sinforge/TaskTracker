using Microsoft.EntityFrameworkCore;

namespace NotificationService.Data;

public class NotificationServiceDbContext : DbContext
{
     public DbSet<Notification> Notifications { get; set; } = null!;
        
     public NotificationServiceDbContext() { }
    
     public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options) : base(options) { }
}