namespace WebApplication1.Data.Auth;

public record LoginRequestUser
{
    public required string? Username { get; set; }
    public required string? Password { get; set; }
}