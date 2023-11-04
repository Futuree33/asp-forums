using Microsoft.EntityFrameworkCore;
using WebApplication1.Database.Models;
using Thread = WebApplication1.Database.Models.Thread;


namespace WebApplication1.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Forum> Forums { get; set; }
    public DbSet<Thread> Threads { get; set; }
}