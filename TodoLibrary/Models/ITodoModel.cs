namespace TodoLibrary.Models
{
    public interface ITodoModel
    {
        int AssignedTo { get; set; }
        int Id { get; set; }
        bool IsComplete { get; set; }
        string? Task { get; set; }
    }
}