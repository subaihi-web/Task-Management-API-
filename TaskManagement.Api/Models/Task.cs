using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Api.Models;

[Table("Tasks")]
public class Task
{
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public long StatusId { get; set; }

    public long UserId { get; set; }

    public Lookup Status { get; set; } = null!;

    public User User { get; set; } = null!;
}
