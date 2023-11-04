using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MimeKit;
using WebApplication1.Data.Attributes;
using WebApplication1.Data.Auth;
using WebApplication1.Database;
using WebApplication1.Helpers;
using WebApplication1.Services;
using Bcrypt = BCrypt.Net.BCrypt;

namespace WebApplication1.Controllers.Auth;

[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly MailService _mailer;
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache _redis;
    
    public AuthController(IConfiguration configuration, DatabaseContext dbContext, MailService mailer, IDistributedCache redis)
    {
        _dbContext = dbContext;
        _mailer = mailer;
        _configuration = configuration;
        _redis = redis;
    }
    
    [HttpPost("login")]
    [Recaptcha]
    public async Task<IActionResult> Login([FromBody] LoginRequestUser basicUser, string recaptchaToken)
    {
        var user = await _dbContext.Users.Where(x => x.Username == basicUser.Username).FirstOrDefaultAsync();

        if (user is null || !Bcrypt.Verify(basicUser.Password, user.Password) || user.Verified == 0)
            return Unauthorized();
        
        HttpContext.Session.SetInt32("user", user.Id);
        HttpContext.Session.SetString("userHash", HttpContext.UserHash());
    
        await _redis.SetStringAsync(user.Id.ToString(), JsonSerializer.Serialize(user), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        });
        
        return Ok();
    }
    
    [HttpPost("register")]
    [Recaptcha]
    public async Task<IActionResult> Register([FromBody] RegisterRequestUser basicUser,  string recaptchaToken)
    {
        var user = (Database.Models.User) basicUser;

        user.Password = Bcrypt.HashPassword(user.Password);
        user.Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(10));
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        await _mailer.Send(basicUser.Email!, $"Welcome to {_configuration["Name"]}", new BodyBuilder
        {
            HtmlBody = $"Welcome {user.Username}, Please <a href=\"{_configuration["Callbacks:VerifyEmail"] + user.Token}\">Click Here</a>, to verify your account!"
        });
        
        return Ok();
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var user = await _dbContext.Users.Where(x => x.Token == token).FirstOrDefaultAsync();
        
        if (user is null || user.Verified == 1)
            return Unauthorized();
        
        user.Verified = 1;
        user.Token = "";

        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ResetPassword(string email)
    {
        var user = await _dbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

        if (user is null || user.Verified == 0)
            return Unauthorized();
        
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(10));

        user.Token = token;

        await _dbContext.SaveChangesAsync();
        await _mailer.Send(user.Email!, "Forgot Password", new BodyBuilder
        {
            HtmlBody = $"Welcome {user.Username}, Please <a href=\"{_configuration["Callbacks:VerifyEmail"] + token}\">Click Here</a>, to reset your password!"
        });
        
        return Ok();
    }
}