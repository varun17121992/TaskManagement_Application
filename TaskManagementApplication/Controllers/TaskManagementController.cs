using Microsoft.AspNetCore.Mvc;
using TaskManagementApplication.Interfaces;

namespace TaskManagementApplication.Controllers;

/// <summary>
/// This controller handles all the API endpoints related to task management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TaskManagementController : ControllerBase
{
    private readonly ITaskManagementService _service;

    public TaskManagementController(ITaskManagementService service)
    {
        _service = service;
    }

    [HttpPost("columns")]
    public IActionResult AddColumn(string name)
    {
        _service.AddColumn(name);
        return Ok();
    }

    [HttpPost("tasks")]
    public IActionResult AddTask(string columnName, string name, string description, DateTime deadline, bool isFavorite)
    {
        var taskId = _service.AddTask(columnName, name, description, deadline, isFavorite);
        return Ok(taskId);
    }

    [HttpPut("tasks/{id}")]
    public IActionResult EditTask(Guid id, string? name, string? description, DateTime? deadline, bool? isFavorite)
    {
        var taskId = _service.EditTask(id, name, description, deadline, isFavorite);
        return Ok(taskId);
    }

    [HttpDelete("tasks/{id}")]
    public IActionResult DeleteTask(Guid id)
    {
        _service.DeleteTask(id);
        return Ok();
    }

    [HttpGet("tasks/{id}")]
    public IActionResult GetTask(Guid id)
    {
        var task = _service.GetTaskDetails(id);
        return Ok(task);
    }

    [HttpPut("tasks/{id}/move")]
    public IActionResult MoveTaskBetweenColumns(Guid id, string toColumn)
    {
        _service.MoveTaskBetweenColumns(id, toColumn);
        return Ok();
    }

    [HttpGet("columns/{columnName}/sorted")]
    public IActionResult GetSortedTasks(string columnName)
    {
        var tasks = _service.GetSortedTasks(columnName);
        return Ok(tasks);
    }
}
