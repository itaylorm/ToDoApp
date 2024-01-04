using TodoLibrary.Models;

namespace TodoLibrary.Data
{
    public interface ITodoDataService
    {
        Task<int> CreateTodo(TodoModel todo);
        Task DeleteToDo(ITodoModel todo);
        Task<ITodoModel?> GetTodo(int assignedTo, int id);
        Task<List<ITodoModel>?> GetTodos(int id);
        Task UpdateTodoComplete(ITodoModel todo);
        Task UpdateTodoTask(ITodoModel todo);
    }
}