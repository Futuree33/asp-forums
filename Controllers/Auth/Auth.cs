using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using WebApplication1.Data.Auth;
using WebApplication1.Database;
using WebApplication1.Database.Models;
using Bcrypt = BCrypt.Net.BCrypt;

namespace WebApplication1.Controllers.Auth;


[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public AuthController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestUser basicUser)
    {
        var user = await _dbContext.Users.Where(pr => pr.Username == basicUser.Username).FirstOrDefaultAsync();

        if (user is null || !Bcrypt.Verify(basicUser.Password, user.Password))
            return Unauthorized();

        HttpContext.Session.SetInt32("user", user.Id);

        return Ok();
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestUser basicUser)
    {
        var user = (User) basicUser;

        user.Password = Bcrypt.HashPassword(user.Password);
        user.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(30));
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
}


