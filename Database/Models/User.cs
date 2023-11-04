namespace WebApplication1.Database.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
    public int? Verified { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? Level { get; set; }
}