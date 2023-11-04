using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication1.Database;
using WebApplication1.Database.Models;

namespace WebApplication1.Services;

public class ForumService
{
    private readonly DatabaseContext _dbContext;
    private IDistributedCache _redis;

    public ForumService(DatabaseContext dbContext, IDistributedCache redis)
    {
        _dbContext = dbContext;
        _redis = redis;
    }
    
    public async Task<Forum?> GetForum(int? id)
    {
        var cachedForum = await _redis.GetAsync("forum-" + id);
        
        if (cachedForum is not null)
            return JsonSerializer.Deserialize<Forum>(Encoding.UTF8.GetString(cachedForum))!;
        
        var forum = await _dbContext.Forums.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (forum is null)
            return null;
        
        var forumJson = JsonSerializer.Serialize(forum);
        
        await _redis.SetStringAsync("forum-" + id, forumJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        });

        return forum;
    }
}