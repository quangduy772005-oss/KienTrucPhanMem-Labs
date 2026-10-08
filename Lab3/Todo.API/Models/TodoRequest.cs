namespace Todo.API.Models;

public class TodoRequest
{
    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
}
