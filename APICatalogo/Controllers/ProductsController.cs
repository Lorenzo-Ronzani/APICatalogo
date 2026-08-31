using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers
{
    // /products
    [Route("[controller]")]
    [ApiController]
    [EnableRateLimiting("fixedwindow")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        /// <summary>
        /// Exibe uma lista de produtos a partir de uma CategoriaId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("products/{id}")]
        public async Task< ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategoryAsync(int id)
        {
            var products = await _uof.ProductRepository.GetProductsByCategoryAsync(id);
            if (products is null)
            {
                return NotFound();
            }
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productsDto);
        }

        private ActionResult<IEnumerable<ProductDTO>> GetProducts(IPagedList<Product> products)
        {
            var metaData = new
            {
                products.Count,
                products.PageSize,
                products.PageCount,
                products.TotalItemCount,
                products.HasNextPage,
                products.HasPreviousPage,
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metaData));

            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDTO);
        }

        [HttpGet("pagination")]
        public async Task< ActionResult<IEnumerable<ProductDTO>>> GetAsync([FromQuery] ProductsParameters productsParameters)
        {
            var products = await _uof.ProductRepository.GetProductsAsync(productsParameters);
            return GetProducts(products);
        }

       

        [HttpGet("filter/price/pagination")]
        public async Task< ActionResult<IEnumerable<ProductDTO>>> GetProductsPriceFilterAsync([FromQuery] FilterProductsPrice filterProductsPrice)
        {
            var products = await _uof.ProductRepository.GetProductsPriceFilterAsync(filterProductsPrice);

            return GetProducts(products);
        }

        //products
        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <returns>Retorna uma lista de objetos Products</returns>
        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task< ActionResult<IEnumerable<ProductDTO>>> GetAsync()
        {
            var products = await _uof.ProductRepository.GetAllAsync();
            if (products is null)
            {
                return NotFound("Products not found");
            }
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productsDto);


        }
        // /products/first
        /// <summary>
        /// Exibe um objeto do primeiro Produto cadastrado
        /// </summary>
        /// <returns></returns>
        [HttpGet("first")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProductDTO>> GetFirstAsync()
        {
            var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == 1);
            if (product is null)
            {
                return NotFound("Product not found");
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
        }
        // products/id
        /// <summary>
        /// Obtem um produto pelo seu identificado productId
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>O objeto do primeiro produto da base de dados /returns>
        [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task< ActionResult<ProductDTO>> GetAsync(int id)
        {
            var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Product not found");
            }
            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
        }

        // /products
        /// <summary>
        /// Inclui um novo Produto
        /// </summary>
        /// <param name="productDto"></param>
        /// <returns>O objeto Product incluido</returns>
        [HttpPost]
        public async Task< ActionResult<ProductDTO>> PostAsync(ProductDTO productDto)
        {
            if (productDto is null)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);

            var newProduct = _uof.ProductRepository.Create(product);
            await _uof.CommitAsync();

            var newProductDto = _mapper.Map<ProductDTO>(newProduct);

            return new CreatedAtRouteResult("GetProduct", new { id = newProductDto.ProductId }, newProductDto);
        }


        [HttpPatch("{id}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task< ActionResult<ProductDTOUpdateResponse>> PatchAsync(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
        {
            if(patchProductDTO is null || id <= 0)
            {
                return BadRequest();
            }
            var product = await _uof.ProductRepository.GetAsync(c => c.ProductId == id);

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
            await _uof.CommitAsync();

            return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
        }

        // /products/id
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProductDTO>> PutAsync(int id, ProductDTO productDto)
        {
            if (id != productDto.ProductId)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);

            var updatedProduct = _uof.ProductRepository.Update(product);
            await _uof.CommitAsync();

            var updatedProductDto = _mapper.Map<ProductDTO>(updatedProduct);
            return Ok(updatedProductDto);
        }

        // products/id
        [HttpDelete("{id:int}")]
        public async Task< ActionResult<ProductDTO>> DeleteAsync(int id)
        {
            var deleted = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
            if (deleted is null)
            {
                return NotFound("Product not found");
            }

            var deletedProduct = _uof.ProductRepository.Delete(deleted);
            await _uof.CommitAsync();

            var deletedProductDto = _mapper.Map<ProductDTO>(deletedProduct);
            return Ok(deletedProductDto);

        }


    }


}
