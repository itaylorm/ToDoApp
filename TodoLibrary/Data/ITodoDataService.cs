using TodoLibrary.Models;

namespace TodoLibrary.Data
{
    public interface ITodoDataService
    {
        Task<int> CreateTodo(int userId, string task);

        Task DeleteTodo(int userId, int id);

        Task<ITodoModel?> GetTodo(int userId, int id);

        Task<List<ITodoModel>?> GetTodos(int userId);

        Task UpdateTodoComplete(int userId, int id);

        Task UpdateTodoTask(int userId, int id, string task);
    }
}