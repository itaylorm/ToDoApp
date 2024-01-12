using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoLibrary.Data;
using TodoLibrary.Models;

namespace TodoApi.Controllers.v1;

[Route("api/v{version:ApiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class TodosController : ControllerBase
{
    private readonly ILogger<TodosController> _log;
    private readonly ITodoDataService _data;

    public TodosController(ILogger<TodosController> log, ITodoDataService data)
    {
        _log = log;
        _data = data;
    }

    private int GetUserId()
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userIdString == null)
        {
            _log.LogError("The user did not have a valid id");
            return -1;
        }
        return int.Parse(userIdString);
    }

    // GET: api/v1/<TodosController>
    /// <summary>
    /// Gets a list of all todos in system
    /// </summary>
    /// <remarks>
    /// Sample Request: GET /api/todos -- 
    /// Sample Response:
    /// [
    ///     {
    ///         "id": 1,
    ///         "task": "Do Laundry"
    ///     },
    ///     {
    ///         "id": 2,
    ///         "name": "Walk Dog"
    ///     }
    /// ]
    /// </remarks>
    /// <returns>List of todos.</returns>
    [HttpGet(Name = "GetAllTodos")]
    public async Task<ActionResult<IEnumerable<TodoModel>>> Get()
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("GET: api/v1/Todos for {userId}", userId);
            var todos = await _data.GetTodos(userId);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The GET call to api/v1/Todos for UserId: {UserId} failed", userId);
            return BadRequest();
        }
    }

    // GET api/v1/Todos/5
    [HttpGet("{todoId}", Name = "GetOneTodo")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("GET: api/v1/Todos/{TodoId} for UserId: {userId}", todoId, userId);
            var todo = await _data.GetTodo(userId, todoId);
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The GET call to {ApiPath} for UserId: {userId} failed. The Id was {TodoId}.",
                userId, "api/v1/Todos/Id", todoId);
            return BadRequest();
        }
    }

    // POST api/v1/Todos
    [HttpPost(Name = "CreateTodo")]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("POST: api/v1/Todos (Task: {Task}) for UserId: {userId}", task, userId);
            int id = await _data.CreateTodo(userId, task);
            var todo = new TodoModel { AssignedTo = userId, Task = task };
            todo.Id = id;
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The POST call to api/v1/Todos for UserId: {userId} failed. The value was {Task}", userId, task);
            return BadRequest();
        }

    }

    // PUT api/v1/Todos/5
    [HttpPut("{todoId}", Name = "UpdateTodoTask")]
    public async Task<IActionResult> Put(int todoId, [FromBody] string task)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("PUT: api/v1/Todos/{todoId} (Task: {task} for UserId: {userId}", todoId, task, userId);
            await _data.UpdateTodoTask(userId, todoId, task);
            var todo = new TodoModel { Id = todoId, AssignedTo = userId, Task = task };
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The PUT call to api/v1/Todos/{todoId} for UserId: {userId} failed. The value was {Task}",
                todoId, userId, task);
            return BadRequest();
        }
    }

    // PUT api/v1/Todos/5/complete
    [HttpPut("{todoId}/complete", Name = "CompleteTodo")]
    public async Task<IActionResult> Complete(int todoId)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("PUT: api/v1/Todos/{todoId}/complete for UserId: {userId}", todoId, userId);
            await _data.UpdateTodoComplete(userId, todoId);
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The PUT call to api/v1/Todos/{todoId}/complete for UserId: {userId} failed", todoId, userId);
            return BadRequest();
        }
    }

    // DELETE api/v1/Todos/5
    [HttpDelete("{todoId}", Name = "DeleteTodo")]
    public async Task<IActionResult> Delete(int todoId)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("DELETE: api/v1/Todos/{todoId}", todoId);
            await _data.DeleteTodo(userId, todoId);
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The DELETE call to api/v1/Todos/{todoId} failed", todoId);
            return BadRequest();
        }
    }
}
