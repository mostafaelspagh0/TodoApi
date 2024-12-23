using Moq;
using TodoApi.Domain.Interfaces;
using TodoApi.Application.Services;
using TodoApi.Application.DTOs;
using TodoApi.Domain.Models;

namespace TodoApi.UnitTests.Services;

public class TodoServiceTests
{
    private readonly Mock<TodoApi.Domain.Interfaces.ITodoRepository> _mockRepository;
    private readonly ITodoService _todoService;

    public TodoServiceTests()
    {
        _mockRepository = new Moq.Mock<ITodoRepository>();
        _todoService = new TodoService(_mockRepository.Object);
    }

    #region CreateTodoAsync Tests

    [Fact]
    public async Task CreateTodoAsync_WithValidDto_ReturnsTodoItem()
    {
        // Arrange
        var dto = new CreateTodoDto { Title = "Test Todo", Description = "Test Description" };
        var expectedTodo = new TodoItem
        {
            Id = 1,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<TodoApi.Domain.Models.TodoItem>()))
            .ReturnsAsync(expectedTodo);

        // Act
        var result = await _todoService.CreateTodoAsync(dto);

        // Assert
        Assert.Equal(expectedTodo.Title, result.Title);
        Assert.Equal(expectedTodo.Description, result.Description);
        Assert.False(result.IsCompleted);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<TodoApi.Domain.Models.TodoItem>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateTodoAsync_WithInvalidTitle_ThrowsArgumentException(string title)
    {
        // Arrange
        var dto = new TodoApi.Application.DTOs.CreateTodoDto { Title = title };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _todoService.CreateTodoAsync(dto));

        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<TodoApi.Domain.Models.TodoItem>()), Times.Never);
    }

    #endregion
    
    #region GetAllTodosAsync Tests
    [Fact]
    public async Task GetAllTodosAsync_ReturnsAllTodos()
    {
        // Arrange
        var expectedTodos = new List<TodoItem>
        {
            new() { Id = 1, Title = "Todo 1" },
            new() { Id = 2, Title = "Todo 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedTodos);

        // Act
        var result = await _todoService.GetAllTodosAsync();

        // Assert
        Assert.Equal(expectedTodos.Count, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllTodosAsync_WhenNoTodos_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TodoItem>());

        // Act
        var result = await _todoService.GetAllTodosAsync();

        // Assert
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
    #endregion
}