using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication1.Data.Attributes;
using WebApplication1.Data.General;
using WebApplication1.Database;
using WebApplication1.Services;
using Thread = WebApplication1.Database.Models.Thread;

namespace WebApplication1.Controllers.User;

[ApiController]
[Route("/api/upload")]
public class Upload : ControllerBase
{
    private readonly UserService _userService;
    private readonly DatabaseContext _dbContext;
    private readonly ForumService _forumService;
    private readonly IDistributedCache _redis;
    
    public Upload(UserService userService, DatabaseContext dbContext, ForumService forumService, IDistributedCache redis)
    {
        _userService = userService;
        _dbContext = dbContext;
        _forumService = forumService;
        _redis = redis;
    }
    
    [HttpPost("create/thread")]
    [AuthorizeSession, Recaptcha]
    public async Task<IActionResult> CreateThread([FromBody] CreateThreadRequest threadRequest, string recaptchaToken)
    {
        if (threadRequest.ParentForum is null)
            return Unauthorized();

        var user = await _userService.GetUser(HttpContext.Session.GetInt32("user"));
        var forum = await _forumService.GetForum(threadRequest.ParentForum);
        
        if (forum?.LevelRequired > user.Level)
            return Unauthorized();
        
        var thread = (Thread)threadRequest;

        thread.Author = user.Id;
        thread.CreatedAt = DateTime.Now;
        thread.UpdatedAt = DateTime.Now;
        
        var cachedThreads = _redis.GetString("threads-" + forum?.Id);
        var threads = cachedThreads is not null
            ? JsonSerializer.Deserialize<List<Thread>>(cachedThreads) 
            : new List<Thread>(); 
        
        _dbContext.Threads.Add(thread);
        await _dbContext.SaveChangesAsync();
        
        threads?.Add(thread);
        
        await _redis.SetStringAsync("threads-" + forum?.Id, JsonSerializer.Serialize(threads), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });
        
        return Ok();
    }
    
    [HttpPost("create/post/{post:int}")]
    [AuthorizeSession]
    public async Task<IActionResult> CreatePost()
    {
        return Ok();
    }
}