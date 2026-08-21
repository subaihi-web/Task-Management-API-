using Microsoft.EntityFrameworkCore;
using TaskEntity = TaskManagement.Api.Models.Task;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Tasks;

namespace TaskManagement.Api.Services;

public sealed class TaskService : ITaskService
{
    private const int TaskStatusMajorCode = 1;

    private readonly AppDbContext _dbContext;

    public TaskService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TaskResponseDto>> GetByCriteriaAsync(long userId, TaskCriteriaDto criteria)
    {
        var query = _dbContext.Tasks
            .AsNoTracking()
            .Include(x => x.Status)
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(criteria.Title))
        {
            var title = criteria.Title.Trim();
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{title}%"));
        }

        if (criteria.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == criteria.StatusId.Value);
        }

        var tasks = await query
            .OrderBy(x => x.ToDate)
            .ThenBy(x => x.Id)
            .ToListAsync();

        return tasks.Select(Map).ToList();
    }

    public async Task<(bool Found, bool Owned, TaskResponseDto? Data)> GetByIdAsync(long userId, long id)
    {
        var task = await _dbContext.Tasks
            .AsNoTracking()
            .Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (task is null)
        {
            return (false, false, null);
        }

        if (task.UserId != userId)
        {
            return (true, false, null);
        }

        return (true, true, Map(task));
    }

    public async Task<(bool Success, string? Error, TaskResponseDto? Data)> AddAsync(long userId, TaskCreateDto request)
    {
        if (!await IsValidTaskStatusAsync(request.StatusId!.Value))
        {
            return (false, "Invalid StatusId. Use a seeded task status lookup.", null);
        }

        var task = new TaskEntity
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            FromDate = request.FromDate!.Value,
            ToDate = request.ToDate!.Value,
            StatusId = request.StatusId.Value,
            UserId = userId
        };

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(task).Reference(x => x.Status).LoadAsync();

        return (true, null, Map(task));
    }

    public async Task<(bool Found, bool Owned, bool Success, string? Error, TaskResponseDto? Data)> UpdateAsync(long userId, TaskUpdateDto request)
    {
        var task = await _dbContext.Tasks
            .Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == request.Id);

        if (task is null)
        {
            return (false, false, false, "Task not found.", null);
        }

        if (task.UserId != userId)
        {
            return (true, false, false, "You can only update tasks assigned to your own user.", null);
        }

        if (!await IsValidTaskStatusAsync(request.StatusId!.Value))
        {
            return (true, true, false, "Invalid StatusId. Use a seeded task status lookup.", null);
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.FromDate = request.FromDate!.Value;
        task.ToDate = request.ToDate!.Value;
        task.StatusId = request.StatusId.Value;

        await _dbContext.SaveChangesAsync();
        await _dbContext.Entry(task).Reference(x => x.Status).LoadAsync();

        return (true, true, true, null, Map(task));
    }

    public async Task<(bool Found, bool Owned, bool Success, string? Error)> DeleteAsync(long userId, long id)
    {
        var task = await _dbContext.Tasks.SingleOrDefaultAsync(x => x.Id == id);

        if (task is null)
        {
            return (false, false, false, "Task not found.");
        }

        if (task.UserId != userId)
        {
            return (true, false, false, "You can only delete tasks assigned to your own user.");
        }

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync();

        return (true, true, true, null);
    }

    private async Task<bool> IsValidTaskStatusAsync(long statusId)
    {
        return await _dbContext.Lookups.AnyAsync(x =>
            x.Id == statusId &&
            x.MajorCode == TaskStatusMajorCode &&
            x.MinorCode >= 1 &&
            x.MinorCode <= 4);
    }

    private static TaskResponseDto Map(TaskEntity task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        FromDate = task.FromDate,
        ToDate = task.ToDate,
        StatusId = task.StatusId,
        Status = task.Status.Name,
        UserId = task.UserId
    };
}
