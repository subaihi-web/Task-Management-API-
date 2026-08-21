using TaskManagement.Api.DTOs.Auth;

namespace TaskManagement.Api.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error, SignupResponseDto? Data)> SignupAsync(SignupRequestDto request);
    Task<(bool Success, string? Error, AuthResponseDto? Data)> LoginAsync(LoginRequestDto request);
}
