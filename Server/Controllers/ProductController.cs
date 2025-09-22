using AutoMapper;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class ProductController(IProductService productService, ILogger<ProductController> logger, IMapper mapper) : ControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductController> _logger = logger;


        [Authorize(Roles = "Admin")]
        [HttpPost("products")]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] CreateProductDTO createProductDto, CancellationToken cancellationToken)
        {
            var productToCreate = _mapper.Map<Product>(createProductDto);

            try
            {
                var createdProduct = await _productService.Create(productToCreate, cancellationToken);
                var productDto = _mapper.Map<ProductDTO>(createdProduct);

                return CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict during product createation");
                return Conflict(new { message = ex.Message });
            }
        }


        //[Authorize(Roles = "Admin")]
        //[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        //[EnableRateLimiting("fixed")]
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllCached(cancellationToken);

            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);

            _logger.LogInformation("Request to retrieve all products.");

            return Ok(productDtos);
        }


        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.GetById(id, cancellationToken);

                var productDto  = _mapper.Map<ProductDTO>(product);

                return Ok(productDto);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Product with ID {id} not found. Exception: {ex}");
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductDTO updateProductDto, CancellationToken cancellationToken)
        {
            try
            {
                var productToUpdate = _mapper.Map<Product>(updateProductDto);

                await _productService.Update(id, productToUpdate, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _productService.Delete(id, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NoContent();
            }
        }
    }
}