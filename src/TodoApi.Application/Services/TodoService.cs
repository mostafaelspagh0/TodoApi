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
}