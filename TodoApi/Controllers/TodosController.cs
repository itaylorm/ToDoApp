using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoLibrary.Data;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
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

    // GET: api/<TodosController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoModel>>> Get()
    {
        int userId = -1;
        try
        {
            _log.LogInformation("GET: api/Todos for {userId}", userId);

            userId = GetUserId();
            var todos = await _data.GetTodos(userId);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The GET call to api/Todos for {UserId} failed", userId);
            return BadRequest();
        }
    }

    // GET api/Todos/5
    [HttpGet("{todoId}")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        try
        {
            _log.LogInformation("GET: api/Todos/{TodoId}", todoId);

            var userId = GetUserId();
            var todo = await _data.GetTodo(userId, todoId);
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The GET call to {ApiPath} failed. The Id was {TodoId}", 
                "api/Todos/Id", todoId);
            return BadRequest();
        }
    }

    // POST api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        try
        {
            _log.LogInformation("POST: api/Todos (Task: {Task})", task);
            var userId = GetUserId();
            int id = await _data.CreateTodo(userId, task);
            var todo = new TodoModel { AssignedTo = userId, Task = task };
            todo.Id = id;
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The POST call to api/Todos failed. The value was {Task}", task);
            return BadRequest();
        }

    }

    // PUT api/Todos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int todoId, [FromBody] string task)
    {
        try
        {
            _log.LogInformation("PUT: api/Todos/{todoId} (Task: {task}", todoId, task);
            var userId = GetUserId();

            await _data.UpdateTodoTask(userId, todoId, task);
            var todo = new TodoModel { Id = todoId, AssignedTo = userId, Task = task };
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The PUT call to api/Todos/{todoId} failed. The value was {Task}", 
                todoId, task);
            return BadRequest();
        }
    }

    // PUT api/Todos/5/complete
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int todoId)
    {
        try
        {
            _log.LogInformation("PUT: api/Todos/{todoId}/complete", todoId);
            var userId = GetUserId();
            await _data.UpdateTodoComplete(userId, todoId);
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The PUT call to api/Todos/{todoId}/complete failed", todoId);
            return BadRequest();
        }
    }

    // DELETE api/Todos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int todoId)
    {
        try
        {
            _log.LogInformation("DELETE: api/Todos/{todoId}", todoId);
            var userId = GetUserId();
            await _data.DeleteTodo(userId, todoId);
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The DELETE call to api/Todos/{todoId} failed", todoId);
            return BadRequest();
        }
    }
}
