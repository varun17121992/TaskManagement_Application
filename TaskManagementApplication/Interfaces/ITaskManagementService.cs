using TaskManagementApplication.Models;

namespace TaskManagementApplication.Interfaces;

public interface ITaskManagementService
{
    void AddColumn(string name);
    Guid AddTask(string columnName, string name, string? description, DateTime deadline, bool isFavorite);
    Guid EditTask(Guid taskId, string? name, string? description, DateTime? deadline, bool? isFavorite);
    void DeleteTask(Guid taskId);
    BoardTask GetTaskDetails(Guid taskId);
    void MoveTaskBetweenColumns(Guid taskId, string toColumn);
    List<BoardTask> GetSortedTasks(string columnName);
}
