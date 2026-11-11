namespace netCurdApi.Models;

public class TodoItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; } = "A";

   
    public int UserId { get; set; }
    public User? User { get; set; } 
}