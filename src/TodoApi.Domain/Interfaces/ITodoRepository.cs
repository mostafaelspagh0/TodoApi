using TodoApi.Domain.Models;

namespace TodoApi.Domain.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem> CreateAsync(TodoItem todo);
}