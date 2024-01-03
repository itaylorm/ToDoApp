using TodoLibrary.Models;

namespace TodoApi.Models
{
    public class TodoDisplayModel : ITodoModel
    {
        public int Id { get; set; }

        public string? Task { get; set; }

        public int AssignedTo { get; set; }

        public bool IsComplete { get; set; }
    }
}
