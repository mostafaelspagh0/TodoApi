using Moq;
using TodoApi.Application.DTOs;
using TodoApi.Application.Services;
using Microsoft.Extensions.Logging;
using TodoApi.API.Controllers;
using TodoApi.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.UnitTests.Controllers;

public class TodoControllerTests
{
    private readonly Mock<ITodoService> _mockService;
    private readonly Mock<ILogger<TodoController>> _mockLogger;
    private readonly TodoController _controller;

    public TodoControllerTests()
    {
        _mockService = new Mock<ITodoService>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<TodoController>>();
        _controller = new TodoController(_mockService.Object, _mockLogger.Object);
    }

    #region CreateTodo Tests

    [Fact]
    public async Task CreateTodo_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateTodoDto { Title = "Test Todo" };
        var createdTodo = new TodoApi.Domain.Models.TodoItem { Id = 1, Title = dto.Title };

        _mockService.Setup(s => s.CreateTodoAsync(dto))
            .ReturnsAsync(createdTodo);

        // Act
        var result = await _controller.CreateTodo(dto);

        // Assert
        var createdAtResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<TodoApi.Domain.Models.TodoItem>(createdAtResult.Value);
        Assert.Equal(createdTodo.Id, returnValue.Id);
    }

    [Fact]
    public async Task CreateTodo_InvalidDto_ReturnsBadRequest()
    {
        // Arrange
        var dto = new TodoApi.Application.DTOs.CreateTodoDto { Title = "" };

        _mockService.Setup(s => s.CreateTodoAsync(dto))
            .ThrowsAsync(new ArgumentException("Title is required"));

        // Act
        var result = await _controller.CreateTodo(dto);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result.Result);
    }

    #endregion
    
    #region GetAllTodos Tests
    [Fact]
    public async Task GetAllTodos_ReturnsOkResultWithTodos()
    {
        // Arrange
        var todos = new List<TodoItem>
        {
            new() { Id = 1, Title = "Todo 1" },
            new() { Id = 2, Title = "Todo 2" }
        };
            
        _mockService.Setup(s => s.GetAllTodosAsync())
            .ReturnsAsync(todos);

        // Act
        var result = await _controller.GetAllTodos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<TodoItem>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }
    #endregion
}