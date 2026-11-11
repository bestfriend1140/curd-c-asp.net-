using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netCurdApi.Data;
using netCurdApi.Models;

namespace netCurdApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/tasks
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _context.TodoItems
            .OrderBy(x => x.Id)
            .ToListAsync();

        return Ok(tasks);
    }

    // GET: api/tasks/todo_01
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var task = await _context.TodoItems.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        return Ok(task);
    }

    [HttpGet("byUserId/{userId}/{status?}")]
    public async Task<IActionResult> GetByUserId(int userId, string? status = null)
    {
        var tasks = await _context.TodoItems
            .Where(t => t.UserId == userId)
            .ToListAsync();

        if (!string.IsNullOrEmpty(status))
        {
            tasks = tasks.Where(t => t.Status == status).ToList();
        }

        return Ok(tasks);
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<IActionResult> Create(TodoItem task)
    {
        var ids = await _context.TodoItems.Select(t => t.Id).ToListAsync();
        var maxIdNumber = 0;

        foreach (var id in ids)
        {
            if (id.StartsWith("todo_", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(id.Substring(5), out var n))
            {
                if (n > maxIdNumber) maxIdNumber = n;
            }
        }

        var nextIdNumber = maxIdNumber + 1;
        task.Id = $"todo_{nextIdNumber:D2}";

        task.CreatedAt = DateTime.Now;

        if (string.IsNullOrEmpty(task.Status))
        {
            task.Status = "A";
        }

        _context.TodoItems.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task
        );
    }

    // PUT: api/tasks/todo_01
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, TodoItem request)
    {
        var task = await _context.TodoItems.FindAsync(id);

        if (task == null) 
            return NotFound(new { message = "Task not found" });

        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.Status = request.Status;

        await _context.SaveChangesAsync();

        return Ok(task);
    }

    // DELETE: api/tasks/todo_01
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var task = await _context.TodoItems.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        _context.TodoItems.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Task deleted successfully" });
    }
}