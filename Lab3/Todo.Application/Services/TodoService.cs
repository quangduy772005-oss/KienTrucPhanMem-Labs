using Todo.Domain.Repositories;
using TodoEntity = Todo.Domain.Entities.Todo;

namespace Todo.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task AddTodo(TodoEntity todo)
    {
        await _repository.AddAsync(todo);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> DeleteTodo(int id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null) return false;

        _repository.DeleteAsync(todo);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<List<TodoEntity>> GetAll()
    {
        var todos = await _repository.GetAllAsync();
        return todos.ToList();
    }

    public async Task<TodoEntity?> GetById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateTodo(TodoEntity todo)
    {
        // item đang được EF tracking -> chỉ cần gán giá trị rồi SaveChanges
        // (code mẫu gọi Update(todo) trên đối tượng mới sẽ gây lỗi trùng key tracking)
        var item = await _repository.GetByIdAsync(todo.Id);
        if (item == null) return false;

        item.Title = todo.Title;
        item.IsCompleted = todo.IsCompleted;
        _repository.UpdateAsync(item);
        await _repository.SaveChangesAsync();
        return true;
    }
}
