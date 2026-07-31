using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {       
        public ProductRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Product> GetProductsByCategory(int id)
        {
            return GetAll().Where(p => p.CategoryId == id);
        }
    }
}
