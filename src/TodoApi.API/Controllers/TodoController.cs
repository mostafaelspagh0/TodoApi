using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.Services;
using TodoApi.Application.DTOs;
using TodoApi.Domain.Models;

namespace TodoApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodo(CreateTodoDto dto)
    {
        try
        {
            var todo = await _todoService.CreateTodoAsync(dto);
            _logger.LogInformation("Created new todo item with ID {Id}", todo.Id);
            return CreatedAtAction(nameof(GetAllTodos), new { id = todo.Id }, todo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid todo item creation attempt");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAllTodos()
    {
        var todos = await _todoService.GetAllTodosAsync();
        return Ok(todos);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetPendingTodos()
    {
        var todos = await _todoService.GetPendingTodosAsync();
        return Ok(todos);
    }
    
    [HttpPut("{id}/complete")]
    public async Task<ActionResult<TodoItem>> CompleteTodo(int id)
    {
        try
        {
            var todo = await _todoService.MarkAsCompletedAsync(id);
            _logger.LogInformation("Marked todo item {Id} as completed", id);
            return Ok(todo);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Attempt to complete non-existent todo item {Id}", id);
            return NotFound(ex.Message);
        }
    }
}