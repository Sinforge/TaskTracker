using System.ComponentModel.DataAnnotations;

namespace AuthService.Data;

public class User
{
    [Key]
    public required Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string Login { get; set; }
    
    [MaxLength(30)]
    public required string Name { get; set; }
    
    [MaxLength(30)]
    public required string Surname { get; set; }
    
    [MaxLength(64)]
    public required string PasswordHash { get; set; }
}