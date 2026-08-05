using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParameters)
        {
            var categories = await GetAllAsync();
            var sortedCategories = categories.OrderBy(p => p.CategoryId).AsQueryable();

            //var result = PagedList<Category>.ToPagedList(sortedCategories, categoriesParameters.PageNumber, categoriesParameters.PageSize);

            var result = await sortedCategories.ToPagedListAsync(categoriesParameters.PageNumber, categoriesParameters.PageSize);
            return result;
        }

        public async Task<IPagedList<Category>> GetCategoriesNameFilterAsync(FilterCategoriesName categoriesParams)
        {
            var categories = await GetAllAsync();
            if (!string.IsNullOrEmpty(categoriesParams.Name))
            {
                categories = categories.Where(c => c.Name.Contains(categoriesParams.Name));
            }

            //var filteredCategories = PagedList<Category>.ToPagedList(categories.AsQueryable(), categoriesParams.PageNumber, categoriesParams.PageSize);

            var filteredCategories = await categories.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);
            return filteredCategories;
        }
    }
}
