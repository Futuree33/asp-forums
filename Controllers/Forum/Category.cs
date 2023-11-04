using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication1.Database;
using WebApplication1.Database.Models;

namespace WebApplication1.Controllers.Forum;

[ApiController]
[Route("/api/categories")]
public class CategoryController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly IDistributedCache _redis;
    
    public CategoryController(DatabaseContext dbContext, IDistributedCache redis)
    {
        _dbContext = dbContext;
        _redis = redis; 
    }

    [HttpGet]
    public async Task<IActionResult> FetchCategories()
    {
        var cachedCategories = await _redis.GetAsync("categories");
        
        // if (cachedCategories is not null && cachedCategories.Length is not 0)
        //     return Ok(JsonSerializer.Deserialize<List<Category>>(Encoding.UTF8.GetString(cachedCategories)));
        
        var categories = await _dbContext.Categories.ToListAsync();
        
        await _redis.SetAsync("categories", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(categories)), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });
        
        return Ok(categories);
    } 
    
    [HttpGet("{category:int}/forums")]
    public async Task<IActionResult> FetchForums(int category)
    {
        var cachedForums = await _redis.GetAsync("forums-" + category);
        
        if (cachedForums is not null)
            return Ok(JsonSerializer.Deserialize<List<Database.Models.Forum>>(Encoding.UTF8.GetString(cachedForums)));
        
        var forums = await _dbContext.Forums.Where(x => x.ParentCategory == category).ToListAsync();

        await _redis.SetAsync("forums-" + category, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(forums)), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });
        
        return Ok(forums);
    }
}