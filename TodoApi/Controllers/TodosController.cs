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
        try
        {
            var userId = GetUserId();
            var todos = await _data.GetTodos(userId);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to get todos");
            return BadRequest();
        }

    }

    // GET api/Todos/5
    [HttpGet("{todoId}")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        try
        {
            var userId = GetUserId();
            var todo = await _data.GetTodo(userId, todoId);
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to get todos");
            return BadRequest();
        }
    }

    // POST api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        try
        {
            var userId = GetUserId();
            int id = await _data.CreateTodo(userId, task);
            var todo = new TodoModel { AssignedTo = userId, Task = task };
            todo.Id = id;
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to get todos");
            return BadRequest();
        }

    }

    // PUT api/Todos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] string task)
    {
        try
        {
            var userId = GetUserId();

            await _data.UpdateTodoTask(userId, id, task);
            var todo = new TodoModel { Id = id, AssignedTo = userId, Task = task };
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to get todos");
            return BadRequest();
        }
    }

    // PUT api/Todos/5/complete
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var userId = GetUserId();
            await _data.UpdateTodoComplete(userId, id);
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to set todo with id {id} to complete", id);
            return BadRequest();
        }
    }

    // DELETE api/Todos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetUserId();
            await _data.DeleteTodo(userId, id);
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to delete todo with id: {id}", id);
            return BadRequest();
        }
    }
}
