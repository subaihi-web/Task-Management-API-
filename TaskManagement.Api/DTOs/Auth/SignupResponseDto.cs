namespace TaskManagement.Api.DTOs.Auth;

public sealed class SignupResponseDto
{
    public long UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
