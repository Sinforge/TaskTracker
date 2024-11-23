using System.ComponentModel.DataAnnotations;

namespace TaskService.Data;

public class Task
{
    [Key]
    public required Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required DateTime CreatingTimestampUtc { get; set; }
    
    public required Guid CreatorId { get; set; }
}