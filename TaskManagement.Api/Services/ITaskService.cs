using TaskManagement.Api.DTOs.Tasks;

namespace TaskManagement.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponseDto>> GetByCriteriaAsync(long userId, TaskCriteriaDto criteria);
    Task<(bool Found, bool Owned, TaskResponseDto? Data)> GetByIdAsync(long userId, long id);
    Task<(bool Success, string? Error, TaskResponseDto? Data)> AddAsync(long userId, TaskCreateDto request);
    Task<(bool Found, bool Owned, bool Success, string? Error, TaskResponseDto? Data)> UpdateAsync(long userId, TaskUpdateDto request);
    Task<(bool Found, bool Owned, bool Success, string? Error)> DeleteAsync(long userId, long id);
}
