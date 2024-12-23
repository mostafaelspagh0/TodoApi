using TodoApi.Application.DTOs;
using TodoApi.Domain.Models;

namespace TodoApi.Application.Services;

public interface ITodoService
{
    public Task<TodoItem> CreateTodoAsync(CreateTodoDto dto);
}