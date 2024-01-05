﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoLibrary.Data;
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
    [HttpGet(Name = "GetAllTodos")]
    public async Task<ActionResult<IEnumerable<TodoModel>>> Get()
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("GET: api/Todos for {userId}", userId);
            var todos = await _data.GetTodos(userId);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The GET call to api/Todos for UserId: {UserId} failed", userId);
            return BadRequest();
        }
    }

    // GET api/Todos/5
    [HttpGet("{todoId}", Name ="GetOneTodo")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("GET: api/Todos/{TodoId} for UserId: {userId}", todoId, userId);
            var todo = await _data.GetTodo(userId, todoId);
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The GET call to {ApiPath} for UserId: {userId} failed. The Id was {TodoId}.", 
                userId, "api/Todos/Id", todoId);
            return BadRequest();
        }
    }

    // POST api/Todos
    [HttpPost(Name = "CreateTodo")]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("POST: api/Todos (Task: {Task}) for UserId: {userId}", task, userId);
            int id = await _data.CreateTodo(userId, task);
            var todo = new TodoModel { AssignedTo = userId, Task = task };
            todo.Id = id;
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The POST call to api/Todos for UserId: {userId} failed. The value was {Task}", userId, task);
            return BadRequest();
        }

    }

    // PUT api/Todos/5
    [HttpPut("{todoId}", Name = "UpdateTodoTask")]
    public async Task<IActionResult> Put(int todoId, [FromBody] string task)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("PUT: api/Todos/{todoId} (Task: {task} for UserId: {userId}", todoId, task, userId);
            await _data.UpdateTodoTask(userId, todoId, task);
            var todo = new TodoModel { Id = todoId, AssignedTo = userId, Task = task };
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The PUT call to api/Todos/{todoId} for UserId: {userId} failed. The value was {Task}", 
                todoId, userId, task);
            return BadRequest();
        }
    }

    // PUT api/Todos/5/complete
    [HttpPut("{todoId}/complete", Name = "CompleteTodo")]
    public async Task<IActionResult> Complete(int todoId)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("PUT: api/Todos/{todoId}/complete for UserId: {userId}", todoId, userId);
            await _data.UpdateTodoComplete(userId, todoId);
            return Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "The PUT call to api/Todos/{todoId}/complete for UserId: {userId} failed", todoId, userId);
            return BadRequest();
        }
    }

    // DELETE api/Todos/5
    [HttpDelete("{todoId}", Name ="DeleteTodo")]
    public async Task<IActionResult> Delete(int todoId)
    {
        int userId = -1;
        try
        {
            userId = GetUserId();
            _log.LogInformation("DELETE: api/Todos/{todoId}", todoId);
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
