using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication1.Database;
using WebApplication1.Database.Models;

namespace WebApplication1.Services;

public class UserService
{
    private readonly DatabaseContext _dbContext;
    private readonly IDistributedCache _redis;
    
    public UserService(DatabaseContext dbContext, IDistributedCache redis)
    {
        _dbContext = dbContext;
        _redis = redis;
    }
    
    public async Task<User> GetUser(int? id)
    {
        var cachedUser = await _redis.GetAsync(id.ToString()!);

        if (cachedUser is not null)
            return JsonSerializer.Deserialize<User>(Encoding.UTF8.GetString(cachedUser));
        
        var user = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        var userJson = JsonSerializer.Serialize(user);
        
        await _redis.SetStringAsync(id.ToString()!, userJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        });

        return user;
    }
}