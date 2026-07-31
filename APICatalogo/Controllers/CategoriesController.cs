using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;

        public CategoriesController(IUnitOfWork uof, IConfiguration configuration)
        {
            _uof = uof;
            _configuration = configuration;

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
        public ActionResult<IEnumerable<Category>> GetProductCategories()
        {
            //return _context.Categories.Include(p => p.Products).AsNoTracking().ToList();
            var categories = _uof.CategoryRepository.GetAll();
            return Ok(categories);

        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDTO>> Get()
        {
            var categories = _uof.CategoryRepository.GetAll();

            if (categories is null)
            {
                return NotFound("Category not found");
            }

            var categoriesDto = categories.ToCategoryDTOList();

            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        public ActionResult<CategoryDTO> Get(int id)
        {

            //throw new Exception("Exception trying to return the category by id");

            var category = _uof.CategoryRepository.Get(c => c.CategoryId == id);
            if (category is null)
            {
                return NotFound("Category not found");
            }

            var categoryDto = category.ToCategoryDTO();
            return Ok(categoryDto);
        }

        [HttpPost]
        public ActionResult<CategoryDTO> Post(CategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto is null)
                {
                    return BadRequest();
                }

                var category = categoryDto.ToCategory();

                var newCategory = _uof.CategoryRepository.Create(category);
                _uof.Commit();
                
                var newCategoryDto = newCategory.ToCategoryDTO();

                return new CreatedAtRouteResult("GetCategory", new { id = newCategoryDto.CategoryId }, newCategoryDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult<CategoryDTO> Put(int id, CategoryDTO categoryDto)
        {
            try
            {
                if (id != categoryDto.CategoryId)
                {
                    return BadRequest();
                }

                var category = categoryDto.ToCategory();

                var updatedCategory = _uof.CategoryRepository.Update(category);
                _uof.Commit();


                var updatedCategoryDto = updatedCategory.ToCategoryDTO();   

                return Ok(updatedCategoryDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoryDTO> Delete(int id)
        {
            try
            {
                var category = _uof.CategoryRepository.Get(c => c.CategoryId == id);
                if (category is null)
                {
                    return NotFound();
                }

                var excludedCategory = _uof.CategoryRepository.Delete(category);
                _uof.Commit();

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
