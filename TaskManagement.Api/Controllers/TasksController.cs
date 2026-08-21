using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Extensions;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("GetByCriteria")]
    [ProducesResponseType(typeof(IReadOnlyList<TaskResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<TaskResponseDto>>> GetByCriteria([FromQuery] TaskCriteriaDto criteria)
    {
        var userId = User.GetUserId();
        var tasks = await _taskService.GetByCriteriaAsync(userId, criteria);
        return Ok(tasks);
    }

    [HttpGet("GetById/{id:long}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponseDto>> GetById(long id)
    {
        var userId = User.GetUserId();
        var result = await _taskService.GetByIdAsync(userId, id);

        if (!result.Found)
        {
            return NotFound(new { message = "Task not found." });
        }

        if (!result.Owned)
        {
            return BadRequest(new { message = "The task is assigned to another user." });
        }

        return Ok(result.Data);
    }

    [HttpPost("Add")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponseDto>> Add([FromBody] TaskCreateDto request)
    {
        var userId = User.GetUserId();
        var result = await _taskService.AddAsync(userId, request);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpPut("Update")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponseDto>> Update([FromBody] TaskUpdateDto request)
    {
        var userId = User.GetUserId();
        var result = await _taskService.UpdateAsync(userId, request);

        if (!result.Found)
        {
            return NotFound(new { message = result.Error });
        }

        if (!result.Owned)
        {
            return BadRequest(new { message = result.Error });
        }

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpDelete("Delete/{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = User.GetUserId();
        var result = await _taskService.DeleteAsync(userId, id);

        if (!result.Found)
        {
            return NotFound(new { message = result.Error });
        }

        if (!result.Owned)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }
}
