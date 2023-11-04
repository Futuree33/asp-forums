using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Auth;
using WebApplication1.Database;
using WebApplication1.Database.Models;
using Thread = WebApplication1.Database.Models.Thread;

namespace WebApplication1.Controllers.Forum;

[ApiController]
[Route("/api/general")]
public class General : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    
    public General(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public enum SearchType
    {
        Users = 0,
        Threads = 1
    }
    
    [HttpGet("search/{query}/{searchType:int}")]
    public async Task<IActionResult> Search(string query, SearchType searchType)
    {
        switch (searchType)
        {
            case SearchType.Threads:
                return Ok(await _dbContext.Threads.Where(x => EF.Functions.Like(x.Name, $"%{query}%")).ToListAsync());
            case SearchType.Users:
            {
                var users = await _dbContext.Users.Where(x => EF.Functions.Like(x.Username, $"%{query}%")).ToListAsync();
                var basicUsers = users.Select(x => (PublicUser) x).ToList();
                
                return Ok(basicUsers);
            }
            default:
                return BadRequest();
        }
    }
}