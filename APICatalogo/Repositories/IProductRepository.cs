using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams);
        //IEnumerable<Product> GetProducts(ProductsParameters productsParams);
        Task<IPagedList<Product>> GetProductsPriceFilterAsync(FilterProductsPrice filterProductsParams);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);

            
    }
}
