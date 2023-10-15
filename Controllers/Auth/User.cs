using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    public UserController(DatabaseContext dbContext, MailService mailService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _mailService = mailService;
        _configuration = configuration;
    }
    
    [HttpGet]
    [AuthorizeSession]
    public async Task<IActionResult> FetchUser()
    {
        int? id = HttpContext.Session.GetInt32("user");

        var user = (BasicUser)await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        
        return Ok(user);
    }
    
    [HttpPost("reset-password")]
    [AuthorizeSession]
    public async Task<IActionResult> ResetPassword()
    {
        int? id = HttpContext.Session.GetInt32("user");

        var user = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (user is null)
            return Unauthorized();
        
        string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(10));

        user.Token = token;

        await _dbContext.SaveChangesAsync();
        await _mailService.SendAsync(user.Email, "Reset Password", new BodyBuilder
        {
            HtmlBody = $"Welcome {user.Username}, Please <a href=\"{_configuration["Callbacks:VerifyEmail"] + token}\">Click Here</a>, to reset your passwqord!"
        });
        
        return Ok();
    }
}