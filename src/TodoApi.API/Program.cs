using Microsoft.EntityFrameworkCore;
using Serilog;

namespace TodoApi.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TodoApi.Infrastructure.Data.TodoDbContext>(options =>
                options.UseInMemoryDatabase("TodoDb"));

            builder.Services.AddScoped<TodoApi.Domain.Interfaces.ITodoRepository, TodoApi.Infrastructure.Repositories.TodoRepository>();
            builder.Services.AddScoped<TodoApi.Application.Services.ITodoService, TodoApi.Application.Services.TodoService>();

            builder.Host.UseSerilog((context, config) =>
                config.WriteTo.Console());

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}