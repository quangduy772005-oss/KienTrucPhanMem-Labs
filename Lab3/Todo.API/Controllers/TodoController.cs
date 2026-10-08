using Microsoft.AspNetCore.Mvc;
using Todo.API.Models;
using Todo.Application.Services;
using TodoEntity = Todo.Domain.Entities.Todo;

namespace Todo.API.Controllers;

[Route("api/v1/todos")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    // GET api/v1/todos
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _todoService.GetAll());
    }

    // GET api/v1/todos/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var todo = await _todoService.GetById(id);
        return todo == null ? NotFound() : Ok(todo);
    }

    // POST api/v1/todos
    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] TodoRequest request)
    {
        var error = Validate(request);
        if (error != null) return BadRequest(error);

        var todo = new TodoEntity
        {
            Title = request.Title!.Trim(),
            IsCompleted = request.IsCompleted
        };
        await _todoService.AddTodo(todo);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    // PUT api/v1/todos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TodoRequest request)
    {
        var error = Validate(request);
        if (error != null) return BadRequest(error);

        var ok = await _todoService.UpdateTodo(new TodoEntity
        {
            Id = id,
            Title = request.Title!.Trim(),
            IsCompleted = request.IsCompleted
        });
        if (!ok) return NotFound();

        return Ok(await _todoService.GetById(id));
    }

    // DELETE api/v1/todos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _todoService.DeleteTodo(id);
        return ok ? NoContent() : NotFound();
    }

    private static string? Validate(TodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title)) return "Title không được để trống.";
        if (request.Title.Trim().Length > 50) return "Title tối đa 50 ký tự.";
        return null;
    }
}
