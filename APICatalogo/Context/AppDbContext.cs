using Microsoft.EntityFrameworkCore;
using APICatalogo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace APICatalogo.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<APICatalogo.Models.Product> Product { get; set; } = default!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Category>? Categories { get; set; } 
        public DbSet<Product>? Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
