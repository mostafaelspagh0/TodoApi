using TodoApi.Domain.Interfaces;
using TodoApi.Domain.Models;
using TodoApi.Application.DTOs;

namespace TodoApi.Application.Services;

public class TodoService(ITodoRepository _repository) : ITodoService
{
    public async Task<TodoItem> CreateTodoAsync(CreateTodoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required");

        var todo = new TodoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false
        };

        return await _repository.CreateAsync(todo);
    }

    public async Task<IEnumerable<TodoItem>> GetAllTodosAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetPendingTodosAsync()
    {
        return await _repository.GetPendingAsync();
    }

    public async Task<TodoItem> MarkAsCompletedAsync(int id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
            throw new KeyNotFoundException($"Todo item with ID {id} not found");

        todo.IsCompleted = true;
        return await _repository.UpdateAsync(todo);
    }
}