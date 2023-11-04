using Mapster;
using WebApplication1.Database.Models;

namespace WebApplication1.Data.Auth;

[AdaptFrom(typeof(User))]
public class PublicUser
{
    public int? Id { get; set; }
    public string? Username { get; set; }
    public int? Level { get; set; }

    public static explicit operator PublicUser(User user) => user.Adapt<PublicUser>(); 
}