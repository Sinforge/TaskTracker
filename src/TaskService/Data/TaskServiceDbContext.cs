using Microsoft.EntityFrameworkCore;

namespace TaskService.Data;

public class TaskServiceDbContext : DbContext
{
    public DbSet<Task> Tasks { get; set; } = null!;
    
    public TaskServiceDbContext() { }

    public TaskServiceDbContext(DbContextOptions<TaskServiceDbContext> options) : base(options) { }
    
}