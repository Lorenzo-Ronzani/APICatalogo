using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        PagedList<Product> GetProducts(ProductsParameters productsParams);
        //IEnumerable<Product> GetProducts(ProductsParameters productsParams);
        PagedList<Product> GetProductsPriceFilter(FilterProductsPrice filterProductsParams);
        IEnumerable<Product> GetProductsByCategory(int id);

            
    }
}
