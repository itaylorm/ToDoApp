namespace TodoLibrary.Models;

public class TodoModel
{
    public int Id { get; set; }

    public string? Task { get; set; }

    public int AssignedTo { get; set; }

    public bool IsComplete { get; set; }
}
