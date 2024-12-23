using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Application.DTOs;
using TodoApi.Domain.Models;
using TodoApi.Infrastructure.Data;

namespace TodoApi.E2ETests;

public class TodoControllerTests : IClassFixture<WebApplicationFactory<TodoApi.API.Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<TodoApi.API.Program> _factory;
    private readonly HttpClient _client;

    public TodoControllerTests(WebApplicationFactory<TodoApi.API.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing db context registration
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<TodoDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add a test database
                services.AddDbContext<TodoDbContext>(options => { options.UseInMemoryDatabase("TodoDbForTesting"); });
            });
        });

        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Clean database before each test
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task CreateTodo_WithValidData_ReturnsCreatedTodo()
    {
        // Arrange
        var newTodo = new CreateTodoDto
        {
            Title = "Test Todo",
            Description = "Test Description",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todo", newTodo);
        var todoItem = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(todoItem);
        Assert.Equal(newTodo.Title, todoItem.Title);
        Assert.Equal(newTodo.Description, todoItem.Description);
        Assert.False(todoItem.IsCompleted);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task CreateTodo_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidTodo = new CreateTodoDto
        {
            Title = "", // Invalid empty title
            Description = "Test Description",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todo", invalidTodo);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task GetAllTodos_ReturnsAllTodos()
    {
        // Arrange
        var todo1 = new CreateTodoDto { Title = "Todo 1", Description = "Description 1" };
        var todo2 = new CreateTodoDto { Title = "Todo 2", Description = "Description 2" };

        await _client.PostAsJsonAsync("/api/todo", todo1);
        await _client.PostAsJsonAsync("/api/todo", todo2);

        // Act
        var response = await _client.GetAsync("/api/todo");
        var todos = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItem>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(todos);
        Assert.Equal(2, todos.Count());
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task GetPendingTodos_ReturnsOnlyPendingTodos()
    {
        // Arrange
        var todo1 = new CreateTodoDto { Title = "Todo 1", Description = "Description 1" };
        var todo2 = new CreateTodoDto { Title = "Todo 2", Description = "Description 2" };

        var response1 = await _client.PostAsJsonAsync("/api/todo", todo1);
        var response2 = await _client.PostAsJsonAsync("/api/todo", todo2);

        var createdTodo1 = await response1.Content.ReadFromJsonAsync<TodoItem>();
        await _client.PutAsync($"/api/todo/{createdTodo1.Id}/complete", null);

        // Act
        var response = await _client.GetAsync("/api/todo/pending");
        var todos = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItem>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(todos);
        Assert.Single(todos);
        Assert.False(todos.First().IsCompleted);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task CompleteTodo_WithValidId_ReturnsTodoWithCompletedStatus()
    {
        // Arrange
        var newTodo = new CreateTodoDto { Title = "Test Todo", Description = "Test Description" };
        var createResponse = await _client.PostAsJsonAsync("/api/todo", newTodo);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        // Act
        var response = await _client.PutAsync($"/api/todo/{createdTodo.Id}/complete", null);
        var completedTodo = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(completedTodo);
        Assert.True(completedTodo.IsCompleted);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task CompleteTodo_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.PutAsync("/api/todo/999/complete", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}