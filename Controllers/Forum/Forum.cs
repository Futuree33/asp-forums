using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication1.Database;
using WebApplication1.Services;
using Thread = WebApplication1.Database.Models.Thread;

namespace WebApplication1.Controllers.Forum;

[ApiController]
[Route("/api/forums")]
public class Forum : ControllerBase
{ 
    private readonly DatabaseContext _dbContext;
    private readonly IDistributedCache _redis;
    private readonly ForumService _forumService;
    
    public Forum(DatabaseContext dbContext, IDistributedCache redis, ForumService forumService)
    {
        _dbContext = dbContext;
        _redis = redis;
        _forumService = forumService;
    }
        
   [HttpGet("{forumId:int}/threads/{from:int}")]
   public async Task<IActionResult> FetchThreads(int? forumId, int from)
   {
       var cachedThreads = await _redis.GetAsync("threads-" + forumId);

       if (cachedThreads is not null)
       {
           var threadList = JsonSerializer.Deserialize<List<Thread>>(cachedThreads);

           var threadListTake= threadList?.Skip(from).Take(10);

           if (threadListTake?.Count() != 0)
               return Ok(threadListTake);
       }
       
       return Ok(await _dbContext.Threads.Where(x => x.ParentForum == forumId).Skip(from).Take(10).ToListAsync());
   }
        
    [HttpGet("{forum:int}/thread/{threadId:int}")]
    public async Task<IActionResult> FetchThread(int forum, int threadId)
    {
        var thread = await _dbContext.Threads.Where(x => x.ParentForum == forum && x.Id == threadId).FirstOrDefaultAsync();
        
        return thread is null 
            ? NotFound()
            : Ok(thread);
    }
}