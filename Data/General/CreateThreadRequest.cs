using System.ComponentModel.DataAnnotations;
using Mapster;
using WebApplication1.Data.Attributes;
using Thread = WebApplication1.Database.Models.Thread;

namespace WebApplication1.Data.General;

[AdaptFrom(typeof(Thread))]
public class CreateThreadRequest
{
    [MinLength(5), MaxLength(40), RegularExpression(@"^[a-zA-Z0-9_]+$")]
    public required string? Name { get; set; }
    
    [MinLength(20), MaxLength(1000)]
    public required string? Content { get; set; }
    
    [Exists("forums", "id")]
    public int? ParentForum { get; set; }
    
    public static explicit operator Thread(CreateThreadRequest createThreadRequest) => createThreadRequest.Adapt<Thread>(); 

}