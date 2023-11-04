using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace WebApplication1.Controllers.Admin;

[ApiController]
[Route("/api/admin/forum-settings")]
public class ForumSettings : ControllerBase
{
    private readonly IDistributedCache _redis;
    
    public ForumSettings(IDistributedCache redis)
    {
        _redis = redis;
    }
}