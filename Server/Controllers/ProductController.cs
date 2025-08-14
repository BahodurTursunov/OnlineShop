using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Cache;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class ProductController(IProductService productService, ILogger<ProductController> logger, IRedisCacheService<Product> cacheService) : ControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly IRedisCacheService<Product> _cacheService = cacheService;
        private readonly ILogger<ProductController> _logger = logger;

        /// <summary>
        /// Create product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                _logger.LogWarning("Attempt to create a product with an empty request body.");
                return BadRequest("Product name cannot be empty.");
            }

            var createdProduct = await _productService.Create(product, cancellationToken);
            _logger.LogInformation($"Product {createdProduct.Name} successfully created.");
            return Ok(new { message = $"Product {product.Name} successfully created" });
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code = "429">Too many requests</response>

        ///[Authorize(Roles = "Admin")]
        //[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        //[EnableRateLimiting("fixed")]
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllCached(cancellationToken);
            _logger.LogInformation("Request to retrieve all products.");
            return Ok(products);
        }

        /// <summary>
        /// Get product with id 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetById(id, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found.");
                return NotFound();
            }

            return Ok(product);
        }


        /// <summary>
        /// Update product by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product, CancellationToken cancellationToken)
        {
            if (product == null)
            {
                _logger.LogWarning("Attempt to update a product with an empty request body.");
                return BadRequest("Product cannot be empty.");
            }

            var updatedProduct = await _productService.Update(id, product, cancellationToken);

            if (updatedProduct == null)
            {
                _logger.LogWarning($"Product with ID {id} not found.");
                return NotFound();
            }

            _logger.LogInformation($"Product with ID {id} was updated.");
            return Ok(updatedProduct);
        }


        /// <summary>
        /// Delete product by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var result = await _productService.Delete(id, cancellationToken);

            if (result is null)
            {
                _logger.LogWarning($"Product with ID {id} not found when attempting to delete.");
                return NotFound();
            }

            return Ok(new
            {
                message = $"Product {result.Name} deleted.",
                id = result.Id
            });
        }
    }
}