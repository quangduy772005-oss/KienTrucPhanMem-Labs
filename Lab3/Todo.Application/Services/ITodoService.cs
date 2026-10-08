using TodoEntity = Todo.Domain.Entities.Todo;

namespace Todo.Application.Services;

public interface ITodoService
{
    Task<List<TodoEntity>> GetAll();
    Task<TodoEntity?> GetById(int id);
    Task AddTodo(TodoEntity todo);
    Task<bool> UpdateTodo(TodoEntity todo);
    Task<bool> DeleteTodo(int id);
}
