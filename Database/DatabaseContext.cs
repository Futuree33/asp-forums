using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Database.Models;

namespace WebApplication1.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {}
    
    public DbSet<User> Users { get; set; }
}