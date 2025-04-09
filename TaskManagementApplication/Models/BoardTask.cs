using System.ComponentModel.DataAnnotations;

namespace TaskManagementApplication.Models;

/// <summary>
/// Represents a task in the task management system.
/// </summary>
public class BoardTask
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFavorite { get; set; }
}



