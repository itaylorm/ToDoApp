using TodoLibrary.Models;

namespace BlazorApiClient.Models
{
    public class TodoDisplayModel : ITodoModel
    {
        public int Id { get; set; }

        public string? Task { get; set; }

        public int AssignedTo { get; set; }

        public bool IsComplete { get; set; }
    }
}
