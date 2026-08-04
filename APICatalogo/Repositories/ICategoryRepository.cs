using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        PagedList<Category> GetCategories(CategoriesParameters categoriesParameters);
        PagedList<Category> GetCategoriesNameFilter(FilterCategoriesName categoriesParams);


    }
}
