using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                _logger.LogWarning("Попытка создать товар с пустым телом запроса.");
                return BadRequest("Название товара не может быть пустым.");
            }

            var createdProduct = await _productService.Create(product, cancellationToken);
            _logger.LogInformation($"Товар {createdProduct.Name} успешно создан");

            //return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            return Ok(new { message = $"Продукт {product.Name} успешно создан" });

        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<Product>> GetAllProducts(CancellationToken cancellationToken)
        {
            var products = _productService.GetAll(cancellationToken);
            _logger.LogInformation("Запрос на получение всех товаров.");

            return Ok(products);

        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetById(id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning($"Товар с ID {id} не найден.");
                return NotFound();
            }

            return Ok(product);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product, CancellationToken cancellationToken)
        {
            if (product == null)
            {
                _logger.LogWarning("Попытка обновить товар с пустым телом запроса.");
                return BadRequest("Товар не может быть пустым.");
            }

            var updatedProduct = await _productService.Update(id, product, cancellationToken);
            if (updatedProduct == null)
            {
                _logger.LogWarning($"Товар с ID {id} не найден.");
                return NotFound();
            }

            _logger.LogInformation($"Товар с ID {id} был обновлен");
            return Ok(updatedProduct);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var result = await _productService.Delete(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning($"Товар с ID {id} не найден при попытке удаления.");
                return NotFound();
            }

            return Ok(new
            {
                message = $"Товар {result.Name} удален",
                id = result.Id
            });
        }
    }
}

