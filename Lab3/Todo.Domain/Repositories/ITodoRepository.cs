using TodoEntity = Todo.Domain.Entities.Todo;

namespace Todo.Domain.Repositories;

public interface ITodoRepository : IRepository<TodoEntity>
{
}
