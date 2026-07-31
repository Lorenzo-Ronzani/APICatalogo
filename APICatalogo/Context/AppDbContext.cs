using Microsoft.EntityFrameworkCore;
using APICatalogo.Models;

namespace APICatalogo.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<APICatalogo.Models.Product> Product { get; set; } = default!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Category>? Categories { get; set; } 
        public DbSet<Product>? Products { get; set; }

    }
}
