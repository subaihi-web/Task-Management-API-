namespace TaskManagement.Api.DTOs.Tasks;

public sealed class TaskResponseDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public long StatusId { get; set; }
    public string Status { get; set; } = string.Empty;
    public long UserId { get; set; }
}
