using Mapster;
using WebApplication1.Database.Models;

namespace WebApplication1.Data.Auth;

[AdaptFrom(typeof(User))]

public record BasicUser
{
    public required int? Id { get; set; }
    public required string? Email { get; set; }
    public required string? Username { get; set; }
    public required string? Token { get; set; }
    public required int? Verified { get; set; }
    public required int? Level { get; set; }
    
    public static explicit operator BasicUser(User user) => user.Adapt<BasicUser>(); 
}