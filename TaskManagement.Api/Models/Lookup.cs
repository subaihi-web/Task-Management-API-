using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models;

public class Lookup
{
    public long Id { get; set; }

    public int MajorCode { get; set; }

    public int MinorCode { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
