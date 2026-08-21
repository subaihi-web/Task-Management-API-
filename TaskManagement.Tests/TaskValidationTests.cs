using System.ComponentModel.DataAnnotations;
using TaskManagement.Api.DTOs.Tasks;
using Xunit;

namespace TaskManagement.Tests;

public class TaskValidationTests
{
    [Fact]
    public void CreateTask_WhenFromDateIsAfterToDate_ShouldFailValidation()
    {
        var dto = new TaskCreateDto
        {
            Title = "Invalid task",
            FromDate = new DateTime(2026, 8, 25),
            ToDate = new DateTime(2026, 8, 24),
            StatusId = 2
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, x => x.ErrorMessage == "FromDate must be less than or equal to ToDate.");
    }

    [Fact]
    public void CreateTask_WhenDatesAreValid_ShouldPassDateValidation()
    {
        var dto = new TaskCreateDto
        {
            Title = "Valid task",
            FromDate = new DateTime(2026, 8, 24),
            ToDate = new DateTime(2026, 8, 25),
            StatusId = 2
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.True(isValid);
    }
}
