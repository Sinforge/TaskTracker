using System.ComponentModel.DataAnnotations;

namespace NotificationService.Data;

public class Notification
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string Message { get; set; }
    
    public required Guid UserId { get; set; }
}