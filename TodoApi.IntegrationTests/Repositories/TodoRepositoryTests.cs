using Microsoft.EntityFrameworkCore;
using TodoApi.Infrastructure.Data;
using TodoApi.Domain.Interfaces;
using TodoApi.Infrastructure.Repositories;

namespace TodoApi.IntegrationTests.Repositories;

public class TodoRepositoryTests
{
    private readonly DbContextOptions<TodoDbContext> _options;
    private readonly TodoDbContext _context;
    private readonly ITodoRepository _repository;

    public TodoRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TodoDbContext(_options);
        _repository = new TodoRepository(_context);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidTodo_ReturnsTodoWithId()
    {
        // Arrange
        var todo = new TodoApi.Domain.Models.TodoItem { Title = "Test Todo" };

        // Act
        var result = await _repository.CreateAsync(todo);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal(todo.Title, result.Title);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingTodo_ReturnsTodo()
    {
        // Arrange
        var todo = new TodoApi.Domain.Models.TodoItem { Title = "Test Todo" };
        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(todo.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todo.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingTodo_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithTodos_ReturnsAllTodos()
    {
        // Arrange
        var todos = new[]
        {
            new TodoApi.Domain.Models.TodoItem { Title = "Todo 1" },
            new TodoApi.Domain.Models.TodoItem { Title = "Todo 2" }
        };
        await _context.TodoItems.AddRangeAsync(todos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNoTodos_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetPendingAsync Tests

    [Fact]
    public async Task GetPendingAsync_WithMixedTodos_ReturnsPendingOnly()
    {
        // Arrange
        var todos = new[]
        {
            new TodoApi.Domain.Models.TodoItem { Title = "Todo 1", IsCompleted = false },
            new TodoApi.Domain.Models.TodoItem { Title = "Todo 2", IsCompleted = true },
            new TodoApi.Domain.Models.TodoItem { Title = "Todo 3", IsCompleted = false }
        };
        await _context.TodoItems.AddRangeAsync(todos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPendingAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, todo => Assert.False(todo.IsCompleted));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ExistingTodo_UpdatesAndReturnsTodo()
    {
        // Arrange
        var todo = new TodoApi.Domain.Models.TodoItem { Title = "Original Title" };
        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync();

        todo.Title = "Updated Title";

        // Act
        var result = await _repository.UpdateAsync(todo);

        // Assert
        Assert.Equal("Updated Title", result.Title);
        var dbTodo = await _context.TodoItems.FindAsync(todo.Id);
        Assert.Equal("Updated Title", dbTodo.Title);
    }

    #endregion
}