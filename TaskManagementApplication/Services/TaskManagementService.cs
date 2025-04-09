using TaskManagementApplication.Interfaces;
using TaskManagementApplication.Models;

namespace TaskManagementApplication.Services;

/// <summary>
/// This service handls everything related to CRUD and various other operations on tasks
/// </summary>
public class TaskManagementService : ITaskManagementService
{
    private readonly List<TaskColumn> _taskColumns = new();
    private readonly ILogger<TaskManagementService> _logger;

    public TaskManagementService(ILogger<TaskManagementService> logger)
    {
        _logger = logger;
    }

    public void AddColumn(string name)
    {
        if (!_taskColumns.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            _taskColumns.Add(new TaskColumn { Name = name });
        }       
    }

    public Guid AddTask(string columnName, string name, string description, DateTime deadline, bool isFavorite)
    {
        try
        {
            var column = _taskColumns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (column == null)
            {
                throw new ArgumentException("Column not found");
            }

            Guid taskId = Guid.NewGuid();
            column.Tasks.Add(new BoardTask { Id = taskId, Name = name, Description = description, Deadline = deadline, IsFavorite = isFavorite });
            return taskId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding task: {Message}", ex.Message);
            throw;
        }   
    }

    public Guid EditTask(Guid taskId, string? name, string? description, DateTime? deadline, bool? isFavorite)
    {
        try
        {
            var boardTask = GetTaskById(taskId);
            if (boardTask == null)
            {
                throw new ArgumentException("BoardTask not found");
            }
            boardTask.Name = name ?? boardTask.Name;
            boardTask.Description = description ?? boardTask.Description;
            boardTask.Deadline = deadline ?? boardTask.Deadline;
            boardTask.IsFavorite = isFavorite ?? boardTask.IsFavorite;

            return taskId;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error editing task: {Message}", "Task not found");
            throw;
        }
        
    }

    public void DeleteTask(Guid taskId)
    {
        try
        {
            foreach (var column in _taskColumns)
            {
                var boardTask = column.Tasks.FirstOrDefault(t => t.Id == taskId);
                if (boardTask == null)
                {
                    throw new ArgumentException("BoardTask not found");                
                }
                column.Tasks.Remove(boardTask);
            }       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task: {Message}", ex.Message);
            throw;
        }
    }

    public BoardTask GetTaskDetails(Guid taskId)
    {
        try
        {
            var boardTask = GetTaskById(taskId);
            if (boardTask == null)
            {
                throw new ArgumentException("BoardTask not found");
            }

            return boardTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task details: {Message}", ex.Message);
            throw;
        }
    }

    public void MoveTaskBetweenColumns(Guid taskId, string toColumn)
    {
        try
        {
            var fromColumn = _taskColumns.FirstOrDefault(c => c.Tasks.Any(t => t.Id == taskId));
            var BoardTask = fromColumn?.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (fromColumn == null || BoardTask == null)
            {
                throw new ArgumentException("BoardTask not found");
            }

            var destColumn = _taskColumns.FirstOrDefault(c => c.Name.Equals(toColumn, StringComparison.OrdinalIgnoreCase));
            if (destColumn == null)
            {
                throw new ArgumentException("Destination column not found");
            }

            fromColumn.Tasks.Remove(BoardTask);
            destColumn.Tasks.Add(BoardTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving task: {Message}", ex.Message);
            throw;
        }
    }

    public List<BoardTask> GetSortedTasks(string columnName)
    {
        try
        {
            var column = _taskColumns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (column == null) throw new ArgumentException("Column not found");

            return column.Tasks
                .OrderByDescending(t => t.IsFavorite)
                .ThenBy(t => t.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sorting tasks: {Message}", ex.Message);
            throw;
        }   
    }

    private BoardTask? GetTaskById(Guid taskId) =>
        _taskColumns.SelectMany(c => c.Tasks).FirstOrDefault(t => t.Id == taskId);
}

