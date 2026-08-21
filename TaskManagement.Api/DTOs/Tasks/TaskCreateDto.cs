using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.DTOs.Tasks;

public sealed class TaskCreateDto : IValidatableObject
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime? FromDate { get; set; }

    [Required]
    public DateTime? ToDate { get; set; }

    [Required]
    public long? StatusId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromDate.HasValue && ToDate.HasValue && FromDate.Value > ToDate.Value)
        {
            yield return new ValidationResult(
                "FromDate must be less than or equal to ToDate.",
                new[] { nameof(FromDate), nameof(ToDate) });
        }
    }
}
