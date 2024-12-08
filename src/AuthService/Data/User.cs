using System.ComponentModel.DataAnnotations;

namespace AuthService.Data;

public class User
{
    [Key]
    public required Guid Id { get; set; }
    
    [MaxLength(100)]
    public string? Login { get; set; }
    
    [MaxLength(30)]
    public string? Name { get; set; }
    
    [MaxLength(30)]
    public string? Surname { get; set; }
    
    [MaxLength(64)]
    public string? PasswordHash { get; set; }
    
    public int? GitHubId { get; set; }
    
    
}