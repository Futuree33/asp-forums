using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MimeKit;
using WebApplication1.Data.Attributes;
using WebApplication1.Data.Auth;
using WebApplication1.Database;
using WebApplication1.Services;

namespace WebApplication1.Controllers.Auth;

[ApiController]
[Route("/api/user")]
public class UserController: ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly MailService _mailService;
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache _redis;
    
    public UserController(DatabaseContext dbContext, MailService mailService, IConfiguration configuration, IDistributedCache redis)
    {
        _dbContext = dbContext;
        _mailService = mailService;
        _configuration = configuration;
        _redis = redis;
    }
    
    [HttpGet]
    [AuthorizeSession]
    public async Task<IActionResult> FetchUser()
    {
        var id = HttpContext.Session.GetInt32("user");

        if (id is null)
            return Unauthorized();
        
        var cachedUser = await _redis.GetAsync(id.ToString()!);

        if (cachedUser is not null)
            return Ok(JsonSerializer.Deserialize<BasicUser>(Encoding.UTF8.GetString(cachedUser)));
        
        var user = (BasicUser) (await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync())!;
        var userJson = JsonSerializer.Serialize(user);
        
        await _redis.SetStringAsync(id.ToString()!, userJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        });
        
        return Ok(user);
    }
    
    [HttpPost("reset-password")]
    [AuthorizeSession]
    public async Task<IActionResult> ResetPassword()
    {
        var id = HttpContext.Session.GetInt32("user");

        var user = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (user is null)
            return Unauthorized();
        
        string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(10));

        user.Token = token;

        await _dbContext.SaveChangesAsync();
        await _mailService.Send(user.Email!, "Reset Password", new BodyBuilder
        {
            HtmlBody = $"Welcome {user.Username}, Please <a href=\"{_configuration["Callbacks:VerifyEmail"] + token}\">Click Here</a>, to reset your passwqord!"
        });
        
        return Ok();
    }
}