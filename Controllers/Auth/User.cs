using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Attributes;
using WebApplication1.Data.Auth;
using WebApplication1.Database;

namespace WebApplication1.Controllers.Auth;

[ApiController]
[Route("/api/user")]
public class UserController: ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public UserController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }


    [HttpGet]
    public async Task<IActionResult> User()
    {
        int? id = HttpContext.Session.GetInt32("user");

        if (id is null)
            return Unauthorized();
        
        var user = (BasicUser) (await _dbContext.Users.Where(pr => pr.Id == id).FirstOrDefaultAsync())!;
        
        return Ok(user);
    }
}