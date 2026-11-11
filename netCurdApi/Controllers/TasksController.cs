
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

    // GET: api/tasks/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _context.TodoItems.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        return Ok(task);
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<IActionResult> Create(TodoItem task)
    {
        task.Id = 0;
        task.CreatedAt = DateTime.Now;

        _context.TodoItems.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task
        );
    }

    // PUT: api/tasks/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TodoItem request)
    {
        var task = await _context.TodoItems.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync();

        return Ok(task);
    }

    // DELETE: api/tasks/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.TodoItems.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        _context.TodoItems.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Task deleted successfully" });
    }
}
