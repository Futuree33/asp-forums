namespace WebApplication1.Database.Models;

public class Thread
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentForum { get; set; }
    public int? Author { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}