using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        var userId = GetUserId();
        var todo = await _data.GetTodo(userId, todoId);
        return Ok(todo);
    }

    // POST api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        var userId = GetUserId();
        var todo = new TodoModel { AssignedTo = userId, Task = task };
        int id = await _data.CreateTodo(todo);
        todo.Id = id;
        return Ok(todo);
    }

    // PUT api/Todos/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] TodoModel todo)
    {
        throw new NotImplementedException();
    }

    // PUT api/Todos/5/complete
    [HttpPut("{id}/complete")]
    public IActionResult Complete(int id)
    {
        throw new NotImplementedException();
    }

    // DELETE api/Todos/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        throw new NotImplementedException();
    }
}
