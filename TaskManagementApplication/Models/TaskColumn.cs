namespace TaskManagementApplication.Models;

/// <summary>
/// Represents a column in the task management system.
/// </summary>
public class TaskColumn
{
    public string Name { get; set; } = string.Empty;
    public List<BoardTask> Tasks { get; set; } = new();
}
