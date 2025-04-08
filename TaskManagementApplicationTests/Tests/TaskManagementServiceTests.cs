using NUnit.Framework;
using TaskManagementApplication.Services;
using TaskManagementApplication.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace TaskManagementApplication.Tests;

[TestFixture]
public class TaskManagementServiceTests
{
    private ITaskManagementService _service;
    private Mock<ILogger<TaskManagementService>> _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<TaskManagementService>>();
        _service = new TaskManagementService(_logger.Object);
        _service.AddColumn("ToDo");
    }

    [Test]
    public void AddColumn_Success()
    {
        //Arrange
        _service.AddColumn("InProgress");

        //Act
        var tasks = _service.GetSortedTasks("InProgress");

        //Assert
        Assert.IsTrue(!tasks.Any());
    }

    [Test]
    public void AddTaskToColumn_Success()
    {
        //Arrange
        Guid taskId = _service.AddTask("ToDo", "Task1", "Description", DateTime.Now.AddDays(1), false);

        //Act
        var task = _service.GetTaskDetails(taskId);

        //Assert
        Assert.IsNotNull(task);
        Assert.AreEqual(task.Id, taskId);
        Assert.AreEqual(task.Name, "Task1");
    }

    [Test]
    public void AddTaskToColumn_Fail()
    {
        // Attempting to add a task to an invalid column
        Assert.Throws<ArgumentException>(()=>_service.AddTask("invalidColumn", "Task1", "Description", DateTime.Now.AddDays(1), false));

        _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error adding task")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Test]
    public void EditTaskDetails_Success()
    {
        //Arrange
        var taskId = _service.AddTask("ToDo", "Task1", "Desccription", DateTime.Now, false);
        var task = _service.GetTaskDetails(taskId);

        //Act
        _service.EditTask(task.Id, "Updated", "Updated Description", DateTime.Now.AddDays(1), true);
        var updated = _service.GetTaskDetails(task.Id);

        //Assert
        Assert.AreEqual("Updated", updated.Name);
        Assert.AreEqual("Updated Description", updated.Description);
        Assert.IsTrue(updated.IsFavorite);
    }

    [Test]
    public void EditTaskDetails_Fail()
    {
        // Attempting to edit task which does not exist
        Assert.Throws<ArgumentException>(() => _service.EditTask(Guid.NewGuid(), "Task1", "Description", DateTime.Now.AddDays(1), false));

        _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error editing task")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Test]
    public void DeleteTask_Successful()
    {
        //Arrange
        var taskId = _service.AddTask("ToDo", "Task1", "Desc", DateTime.Now, false);

        //Act
        _service.DeleteTask(taskId);

        //Assert
        Assert.Throws<ArgumentException>(() => _service.GetTaskDetails(taskId));
    }

    [Test]
    public void DeleteTask_Fail()
    {
        // Attempting to delete task which does not exist
        Assert.Throws<ArgumentException>(() => _service.DeleteTask(Guid.NewGuid()));

        _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error deleting task")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Test]
    public void MoveTaskBetweenColumns_Success()
    {
        //Arrange
        _service.AddColumn("Done");
        var taskId = _service.AddTask("ToDo", "Task1", "Desc", DateTime.Now, false);

        //Act: Moving task from "ToDo" to "Done"
        _service.MoveTaskBetweenColumns(taskId, "Done");

        //Assert
        Assert.That(!_service.GetSortedTasks("ToDo").Any());
        Assert.AreEqual("Task1", _service.GetSortedTasks("Done")[0].Name);
    }

    [Test]
    public void MoveTaskBetweenColumns_Fail()
    {
        var taskId = _service.AddTask("ToDo", "Task1", "Desc", DateTime.Now, false);

        //Trying to move to invalid column
        Assert.Throws<ArgumentException>(()=>_service.MoveTaskBetweenColumns(taskId, "invalidColumn"));

        _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error moving task")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Test]
    public void GetSortedTasks_Success()
    {
        //Arrange
        _service.AddTask("ToDo", "A", "description1", DateTime.UtcNow, false);
        _service.AddTask("ToDo", "B", "description2", DateTime.UtcNow, true);
        _service.AddTask("ToDo", "C", "description3", DateTime.UtcNow, true);

        //Act
        var tasks = _service.GetSortedTasks("ToDo");

        //Assert
        Assert.AreEqual("B", tasks[0].Name);
        Assert.AreEqual("C", tasks[1].Name);
        Assert.AreEqual("A", tasks[2].Name);
    }

    [Test]
    public void GetSortedTasks_Fail()
    {
        // Attempting to sort tasks for an invalid column
        Assert.Throws<ArgumentException>(() => _service.GetSortedTasks("InvalidColumn"));

        _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error sorting tasks")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }
}
