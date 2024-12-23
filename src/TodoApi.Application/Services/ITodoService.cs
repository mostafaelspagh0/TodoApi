using TodoApi.Application.DTOs;
using TodoApi.Domain.Models;

namespace TodoApi.Application.Services;

public interface ITodoService
{
    Task<TodoItem> CreateTodoAsync(CreateTodoDto dto);
    Task<IEnumerable<TodoItem>> GetAllTodosAsync();
    Task<IEnumerable<TodoItem>> GetPendingTodosAsync();
    Task<TodoItem> MarkAsCompletedAsync(int id);
}