namespace WebApplication1.Database.Models;

public class Forum
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentCategory { get; set; }
    public int? LevelRequired { get; set; }
}