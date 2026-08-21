using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models;

public class User
{
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string HashedPassword { get; set; } = string.Empty;

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
