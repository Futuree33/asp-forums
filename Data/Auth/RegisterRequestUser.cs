using System.ComponentModel.DataAnnotations;
using Mapster;
using WebApplication1.Data.Attributes;
using WebApplication1.Database.Models;

namespace WebApplication1.Data.Auth;

[AdaptFrom(typeof(User))]
public record RegisterRequestUser
{
    [MaxLength(20), MinLength(4), Unique("users", "username")]
    public required string? Username { get; set; }
    
    [EmailAddress, Unique("users", "email")]
    public required string? Email { get; set; }
    
    [MinLength(10)]
    public required string? Password { get; set; }
    
    public static explicit operator User(RegisterRequestUser basicUser) => basicUser.Adapt<User>(); 
}