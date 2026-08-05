using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        //public IEnumerable<Product> GetProducts(ProductsParameters productsParams)
        //{
        //      return GetAll()
        //          .OrderBy(p => p.Name)
        //          .Skip((productsParams.PageNumber - 1) * productsParams.PageSize)
        //          .Take(productsParams.PageSize).ToList();
        //}

        public async Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams)
        {
            var products = await GetAllAsync();

            var sortedProducts = products.OrderBy(p => p.ProductId).AsQueryable();

            var result = await sortedProducts.ToPagedListAsync(productsParams.PageNumber, productsParams.PageSize);
            return result;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
        {
            var products = await GetAllAsync();
            var productsCategory = products.Where(p => p.CategoryId == id);
            return productsCategory;
        }

        public async Task<IPagedList<Product>> GetProductsPriceFilterAsync(FilterProductsPrice filterProductsParams)
        {
            var products = await GetAllAsync();
            if (filterProductsParams.Price.HasValue && !string.IsNullOrEmpty(filterProductsParams.CriteriaPrice))
            {
                var priceValue = (double)filterProductsParams.Price.Value; // <- conversão aqui

                if (filterProductsParams.CriteriaPrice.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price > priceValue).OrderBy(p => p.Price);
                }
                else if (filterProductsParams.CriteriaPrice.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price < priceValue).OrderBy(p => p.Price);
                }
                else if (filterProductsParams.CriteriaPrice.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price == priceValue).OrderBy(p => p.Price);
                }
            }

            var filteredProducts = await products.ToPagedListAsync(filterProductsParams.PageNumber, filterProductsParams.PageSize);
            return filteredProducts;
        }
    }
}

