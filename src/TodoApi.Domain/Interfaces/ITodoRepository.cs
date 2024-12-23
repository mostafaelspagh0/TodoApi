using TodoApi.Domain.Models;

namespace TodoApi.Domain.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem> CreateAsync(TodoItem todo);
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<IEnumerable<TodoItem>> GetPendingAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task<TodoItem> UpdateAsync(TodoItem todo);
}