using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Auth;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Services;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(AppDbContext dbContext, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<(bool Success, string? Error, SignupResponseDto? Data)> SignupAsync(SignupRequestDto request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _dbContext.Users.AnyAsync(x => x.Email == email))
        {
            return (false, "Email already exists.", null);
        }

        if (await _dbContext.Users.AnyAsync(x => x.Username == username))
        {
            return (false, "Username already exists.", null);
        }

        var user = new User
        {
            Username = username,
            Email = email,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return (true, null, new SignupResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        });
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Data)> LoginAsync(LoginRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
        {
            return (false, "Invalid email or password.", null);
        }

        return (true, null, new AuthResponseDto
        {
            Token = _jwtTokenService.GenerateToken(user)
        });
    }
}
