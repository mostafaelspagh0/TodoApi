using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.Services;
using TodoApi.Application.DTOs;
using TodoApi.Domain.Models;
using TodoApi.Domain.Interfaces;

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
}