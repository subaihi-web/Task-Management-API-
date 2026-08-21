using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}
