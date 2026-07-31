using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context) { }

    }
}
