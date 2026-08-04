using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context) { }

        public PagedList<Category> GetCategories(CategoriesParameters categoriesParameters)
        {
            var categories = GetAll().OrderBy(p => p.CategoryId).AsQueryable();
            var sortedCategories = PagedList<Category>.ToPagedList(categories, categoriesParameters.PageNumber, categoriesParameters.PageSize);
            return sortedCategories;
        }

        public PagedList<Category> GetCategoriesNameFilter(FilterCategoriesName categoriesParams)
        {
            var categories = GetAll().AsQueryable();
            if (!string.IsNullOrEmpty(categoriesParams.Name))
            {
                categories = categories.Where(c => c.Name.Contains(categoriesParams.Name));
            }

            var filteredCategories = PagedList<Category>.ToPagedList(categories, categoriesParams.PageNumber, categoriesParams.PageSize);

            return filteredCategories;
        }
    }
}
