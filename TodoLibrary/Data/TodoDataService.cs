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

        public async Task<List<ITodoModel>?> GetTodos(int assignedTo)
        {
            var todos = await _sql.LoadDataAsync<TodoModel, dynamic>("dbo.spTodos_GetAllAssigned", 
                new { AssignedTo = assignedTo }, ConnectionStringName); ;
            if (todos != null)
            {
                return todos.ToList<ITodoModel>();
            }
            else
            {
                _log.LogError("Unable to retrieve todos for {id}", assignedTo);
                return null;
            }
        }

        public async Task<ITodoModel?> GetTodo(int assignedTo, int id)
        {
            var todos = await _sql.LoadDataAsync<TodoModel, dynamic>("dbo.spTodos_GetOneAssigned", 
                new { AssignedTo = assignedTo, TodoId = id }, ConnectionStringName); ;
            if (todos != null)
            {
                return todos.FirstOrDefault<ITodoModel>();
            }
            else
            {
                _log.LogError("Unable to retrieve todo for {id} and assignedTo: {assignedTo}", id, assignedTo);
                return null;
            }
        }

        public async Task<int> CreateTodo(TodoModel todo)
        {
            int id = await _sql.AddDataAsync("dbo.spTodos_Create", 
                new { todo.Id, todo.AssignedTo, todo.Task }, "Id", ConnectionStringName);
            if (id != -1)
            {
                _log.LogInformation("Added Todo Task: {Task} AssignedTo: {AssignedTo} with id {Id}", todo.Task, todo.AssignedTo, id);
            }
            else
            {
                _log.LogError("Failed to add Todo Task: {Task} AssignedTo: {AssignedTo}", todo.Task, todo.AssignedTo);
            }
            return id;
        }


        public async Task UpdateTodoTask(ITodoModel todo)
        {
            await _sql.SaveDataAsync("dbo.spTodo_UpdateTask", todo, ConnectionStringName);
        }

        public async Task UpdateTodoComplete(ITodoModel todo)
        {
            await _sql.SaveDataAsync("dbo.spTodo_UpdateComplete", new { todo.AssignedTo, TodoId = todo.Id }, ConnectionStringName);
        }

        public async Task DeleteToDo(ITodoModel todo)
        {
            await _sql.SaveDataAsync("dbo.spTodo_UpdateComplete", new { todo.AssignedTo, TodoId = todo.Id }, ConnectionStringName);
        }
    }
}
