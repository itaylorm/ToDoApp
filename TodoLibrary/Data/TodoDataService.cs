using Microsoft.Extensions.Logging;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;

namespace TodoLibrary.Data
{
    public class TodoDataService : ITodoDataService
    {
        private const string ConnectionStringName = "Default";

        private readonly IDataAccess _sql;
        private readonly ILogger<TodoDataService> _log;

        public TodoDataService(IDataAccess sql, ILogger<TodoDataService> log)
        {
            _sql = sql;
            _log = log;
        }

        public async Task<List<ITodoModel>?> GetTodos(int userId)
        {
            var todos = await _sql.LoadDataAsync<TodoModel, dynamic>("dbo.spTodos_GetAllAssigned", 
                new { AssignedTo = userId }, ConnectionStringName); ;
            if (todos != null)
            {
                return todos.ToList<ITodoModel>();
            }
            else
            {
                _log.LogError("Unable to retrieve todos for {id}", userId);
                return null;
            }
        }

        public async Task<ITodoModel?> GetTodo(int userId, int id)
        {
            var todos = await _sql.LoadDataAsync<TodoModel, dynamic>("dbo.spTodos_GetOneAssigned", 
                new { AssignedTo = userId, TodoId = id }, ConnectionStringName); ;
            if (todos != null)
            {
                return todos.FirstOrDefault<ITodoModel>();
            }
            else
            {
                _log.LogError("Unable to retrieve todo for {id} and assignedTo: {assignedTo}", id, userId);
                return null;
            }
        }

        public async Task<int> CreateTodo(int userId, string task)
        {
            int id = await _sql.AddDataAsync("dbo.spTodos_Create", 
                new { Id = -1, AssignedTo = userId, task }, "Id", ConnectionStringName);
            if (id != -1)
            {
                _log.LogInformation("Added Todo Task: {Task} AssignedTo: {AssignedTo} with id {Id}", task, userId, id);
            }
            else
            {
                _log.LogError("Failed to add Todo Task: {Task} AssignedTo: {AssignedTo}", task, userId);
            }
            return id;
        }


        public async Task UpdateTodoTask(int userId, int id, string task)
        {
            await _sql.SaveDataAsync("dbo.spTodos_UpdateTask", new { AssignedTo = userId, TodoId = id, Task = task }, ConnectionStringName);
        }

        public async Task UpdateTodoComplete(int userId, int id)
        {
            await _sql.SaveDataAsync("dbo.spTodos_UpdateComplete", new { AssignedTo = userId, TodoId = id }, ConnectionStringName);
        }

        public async Task DeleteTodo(int userId, int id)
        {
            await _sql.SaveDataAsync("dbo.spTodos_Delete", new { AssignedTo = userId, TodoId = id }, ConnectionStringName);
        }
    }
}
