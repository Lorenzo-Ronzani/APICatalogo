using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    // /products
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("products/{id}")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsByCategory(int id)
        {
            var products = _uof.ProductRepository.GetProductsByCategory(id);
            if (products is null)
            {
                return NotFound();
            }
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productsDto);
        }

        private ActionResult<IEnumerable<ProductDTO>> GetProducts(PagedList<Product> products)
        {
            var metaData = new
            {
                products.TotalCount,
                products.PageSize,
                products.CurrentPage,
                products.TotalPages,
                products.HasNext,
                products.HasPrevious,
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metaData));

            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDTO);
        }

        [HttpGet("pagination")]
        public ActionResult<IEnumerable<ProductDTO>> Get([FromQuery] ProductsParameters productsParameters)
        {
            var products = _uof.ProductRepository.GetProducts(productsParameters);
            return GetProducts(products);
        }

       

        [HttpGet("filter/price/pagination")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsPriceFilter([FromQuery] FilterProductsPrice filterProductsPrice)
        {
            var products = _uof.ProductRepository.GetProductsPriceFilter(filterProductsPrice);

            return GetProducts(products);
        }

        // /products
        [HttpGet]
        public ActionResult<IEnumerable<ProductDTO>> Get()
        {
            var products = _uof.ProductRepository.GetAll();
            if (products is null)
            {
                return NotFound("Products not found");
            }
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productsDto);


        }
        // /products/first
        [HttpGet("first")]
        public ActionResult<ProductDTO> GetFirst()
        {
            var product = _uof.ProductRepository.Get(p => p.ProductId == 1);
            if (product is null)
            {
                return NotFound("Product not found");
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
        }
        // products/id
        [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
        public ActionResult<ProductDTO> Get(int id)
        {
            var product = _uof.ProductRepository.Get(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Product not found");
            }
            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
        }

        // /products
        [HttpPost]
        public ActionResult<ProductDTO> Post(ProductDTO productDto)
        {
            if (productDto is null)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);

            var newProduct = _uof.ProductRepository.Create(product);
            _uof.Commit();

            var newProductDto = _mapper.Map<ProductDTO>(newProduct);

            return new CreatedAtRouteResult("GetProduct", new { id = newProductDto.ProductId }, newProductDto);
        }


        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProductDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
        {
            if(patchProductDTO is null || id <= 0)
            {
                return BadRequest();
            }
            var product = _uof.ProductRepository.Get(c => c.ProductId == id);

            if (product is null)
            {
                return NotFound();
            }
            var productUpdateRequest = _mapper.Map<ProductDTOUpdateRequest>(product);

            patchProductDTO.ApplyTo(productUpdateRequest, ModelState);

            if(!ModelState.IsValid || !TryValidateModel(productUpdateRequest)){
                return BadRequest(ModelState);
            }

            _mapper.Map(productUpdateRequest, product);

            _uof.ProductRepository.Update(product);
            _uof.Commit();

            return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
        }

        // /products/id
        [HttpPut("{id:int}")]
        public ActionResult<ProductDTO> Put(int id, ProductDTO productDto)
        {
            if (id != productDto.ProductId)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);

            var updatedProduct = _uof.ProductRepository.Update(product);
            _uof.Commit();

            var updatedProductDto = _mapper.Map<ProductDTO>(updatedProduct);
            return Ok(updatedProductDto);
        }

        // products/id
        [HttpDelete("{id:int}")]
        public ActionResult<ProductDTO> Delete(int id)
        {
            var deleted = _uof.ProductRepository.Get(p => p.ProductId == id);
            if (deleted is null)
            {
                return NotFound("Product not found");
            }

            var deletedProduct = _uof.ProductRepository.Delete(deleted);
            _uof.Commit();

            var deletedProductDto = _mapper.Map<ProductDTO>(deletedProduct);
            return Ok(deletedProductDto);

        }


    }


}
