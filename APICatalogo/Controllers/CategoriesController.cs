using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[EnableRateLimiting("fixedwindow")]
    public class CategoriesController : ControllerBase
    {

        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private const string CacheCategoriesKey = "CacheCategories";

        public CategoriesController(IUnitOfWork uof, IConfiguration configuration, IMemoryCache cache)
        {
            _uof = uof;
            _configuration = configuration;
            _cache = cache;
        }

        [HttpGet("readConfigFile")]
        public string GetValores()
        {
            var value1 = _configuration["key1"];
            var value2 = _configuration["key2"];
            var section1 = _configuration["section1:key2"];

            return $"Key1 = {value1} + Key2 = {value2} + Section1 => key2 = {section1}";
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Category>>> GetProductCategoriesAsync()
        {
            if (!_cache.TryGetValue(CacheCategoriesKey, out IEnumerable<Category>? categories))
            {
                categories = await _uof.CategoryRepository.GetAllAsync();

                if (categories is null)
                {
                    return NotFound("Category not found");
                }

                _cache.Set(CacheCategoriesKey, categories, TimeSpan.FromSeconds(30));
            }

            return Ok(categories);
        }
        /// <summary>
        /// Obtem uma lista de objetos Category
        /// </summary>
        /// <returns>Uma lista de objetos Category </returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAsync()
        {
            if (!_cache.TryGetValue(CacheCategoriesKey, out IEnumerable<Category>? categories))
            {
                categories = await _uof.CategoryRepository.GetAllAsync();

                if (categories is null)
                {
                    return NotFound("Category not found");
                }

                _cache.Set(CacheCategoriesKey, categories, TimeSpan.FromSeconds(30));
            }

            var categoriesDto = categories!.ToCategoryDTOList();

            return Ok(categoriesDto);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAsync([FromQuery] CategoriesParameters categoriesParameters)
        {
            var categories = await _uof.CategoryRepository.GetCategoriesAsync(categoriesParameters);
            return GetCategories(categories);
        }

        private ActionResult<IEnumerable<CategoryDTO>> GetCategories(IPagedList<Category> categories)
        {
            var metaData = new
            {
                categories.Count,
                categories.PageSize,
                categories.PageCount,
                categories.TotalItemCount,
                categories.HasNextPage,
                categories.HasPreviousPage,
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metaData));

            var categoriesDTO = categories.ToCategoryDTOList();

            return Ok(categoriesDTO);
        }

        [HttpGet("filter/name/pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesNameFilterAsync([FromQuery] FilterCategoriesName filterCategoriesName)
        {
            var filteredCategories = await _uof.CategoryRepository.GetCategoriesNameFilterAsync(filterCategoriesName);

            return GetCategories(filteredCategories);
        }
        /// <summary>
        /// Obtem uma Category pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objetos Id</returns>
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoryDTO>> GetAsync(int id)
        {

            //throw new Exception("Exception trying to return the category by id");

            var cacheCategoryKey = $"CacheCategory_{id}";

            if (!_cache.TryGetValue(cacheCategoryKey, out Category? category))
            {
                category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

                if (category is null)
                {
                    return NotFound("Category not found");
                }

                _cache.Set(cacheCategoryKey, category, TimeSpan.FromSeconds(30));
            }

            var categoryDto = category!.ToCategoryDTO();

            return Ok(categoryDto);
        }
        /// <summary>
        /// Inclui uma nova Category
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        ///     POST api/categories
        ///     {
        ///         "categoryId": 1,
        ///         "name": "categoria1",
        ///         "imageUlr": "http://teste.net/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoryDto"></param>
        /// <returns>O objeto Category incluido</returns>
        /// <remarks>Retorna o objeto Category inlcuido</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoryDTO>> PostAsync(CategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto is null)
                {
                    return BadRequest();
                }

                var category = categoryDto.ToCategory();

                var newCategory = _uof.CategoryRepository.Create(category);
                await _uof.CommitAsync();

                _cache.Remove(CacheCategoriesKey);

                var newCategoryDto = newCategory.ToCategoryDTO();

                return new CreatedAtRouteResult("GetCategory", new { id = newCategoryDto.CategoryId }, newCategoryDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoryDTO>> PutAsync(int id, CategoryDTO categoryDto)
        {
            try
            {
                if (id != categoryDto.CategoryId)
                {
                    return BadRequest();
                }

                var category = categoryDto.ToCategory();

                var updatedCategory = _uof.CategoryRepository.Update(category);
                await _uof.CommitAsync();

                _cache.Remove(CacheCategoriesKey);
                _cache.Remove($"CacheCategory_{id}");

                var updatedCategoryDto = updatedCategory.ToCategoryDTO();

                return Ok(updatedCategoryDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoryDTO>> DeleteAsync(int id)
        {
            try
            {
                var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);
                if (category is null)
                {
                    return NotFound();
                }

                var excludedCategory = _uof.CategoryRepository.Delete(category);
                await _uof.CommitAsync();

                _cache.Remove(CacheCategoriesKey);
                _cache.Remove($"CacheCategory_{id}");

                var excludedCategoryDto = excludedCategory.ToCategoryDTO();

                return Ok(excludedCategoryDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
