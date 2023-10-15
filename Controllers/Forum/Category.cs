using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Database;

namespace WebApplication1.Controllers.Forum;


[ApiController]
[Route("/api/categories")]
public class CategoryController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public CategoryController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> FetchCategories()
    {
        var categories = await _dbContext.Categories.ToListAsync();
        
        return Ok(categories);
    } 
}