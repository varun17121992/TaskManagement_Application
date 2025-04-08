namespace TaskManagementApplication.Models;

public class TaskColumn
{
    public string Name { get; set; } = string.Empty;
    public List<BoardTask> Tasks { get; set; } = new();
}
