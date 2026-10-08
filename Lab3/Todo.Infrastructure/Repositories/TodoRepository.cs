using Todo.Domain.Repositories;
using Todo.Infrastructure.Data;
using TodoEntity = Todo.Domain.Entities.Todo;

namespace Todo.Infrastructure.Repositories;

public class TodoRepository : Repository<TodoEntity>, ITodoRepository
{
    public TodoRepository(TodoDbContext context) : base(context)
    {
    }
}
