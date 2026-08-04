using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

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

        public PagedList<Product> GetProducts(ProductsParameters productsParams)
        {
            var products = GetAll().OrderBy(p => p.ProductId).AsQueryable();
            var sortedProducts = PagedList<Product>.ToPagedList(products, productsParams.PageNumber, productsParams.PageSize);
            return sortedProducts;
        }

        public IEnumerable<Product> GetProductsByCategory(int id)
        {
            return GetAll().Where(p => p.CategoryId == id);
        }

        public PagedList<Product> GetProductsPriceFilter(FilterProductsPrice filterProductsParams)
        {
            var products = GetAll().AsQueryable();
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

            var filteredProducts = PagedList<Product>.ToPagedList(products, filterProductsParams.PageNumber,
                                                                               filterProductsParams.PageSize);
            return filteredProducts;
        }
    }
}

