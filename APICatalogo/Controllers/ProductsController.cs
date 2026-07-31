using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    // /products
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uof;

        public ProductsController(IUnitOfWork uof)
        {
            _uof = uof;
        }

        [HttpGet("products/{id}")]
        public ActionResult<IEnumerable<Product>> GetProductsByCategory(int id)
        {
            var product = _uof.ProductRepository.GetProductsByCategory(id);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // /products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            var products = _uof.ProductRepository.GetAll();
            if (products is null)
            {
                return NotFound("Products not found");
            }
            return Ok(products);


        }
        // /products/first
        [HttpGet("first")]
        public ActionResult<Product> GetFirst()
        {
            var products = _uof.ProductRepository.Get(p => p.ProductId == 1);
            if (products is null)
            {
                return NotFound("Products not found");
            }
            return products;


        }
        // products/id
        [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
        public ActionResult<Product> Get(int id)
        {
            var product = _uof.ProductRepository.Get(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Product not found");
            }
            return Ok(product);


        }

        // /products
        [HttpPost]
        public ActionResult Post(Product product)
        {
            if (product is null)
            {
                return BadRequest();
            }

            var newProduct = _uof.ProductRepository.Create(product);
            _uof.Commit();  

            return new CreatedAtRouteResult("GetProduct", new { id = newProduct.ProductId }, newProduct);
        }

        // /products/id
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            var updatedProduct = _uof.ProductRepository.Update(product);
            _uof.Commit();
            return Ok(updatedProduct);
        }

        // products/id
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _uof.ProductRepository.Get(p => p.ProductId == id);
            if (deleted is null)
            {
                return NotFound("Product not found");
            }
            var deletedProduct = _uof.ProductRepository.Delete(deleted);
            _uof.Commit();
            return Ok(deletedProduct);
         
        }


    }


}
