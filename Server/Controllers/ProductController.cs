﻿using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("[controller]")]
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
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            try
            {
                if (product.Name == null)
                {
                    _logger.LogWarning("Попытка создать товар с пустым телом запроса.");
                    return BadRequest("Название товара не может быть пустым.");
                }

                var createdProduct = await _productService.Create(product);
                _logger.LogInformation($"Товар {createdProduct.Name} успешно создан");

                //return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
                return Ok(new { message = $"Продукт {product.Name} успешно создан" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании товара.");
                return StatusCode(500, "Не корректно введенные данные");
            }
        }

        [HttpGet("GetAllProducts")]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                var products = _productService.GetAll();
                _logger.LogInformation("Запрос на получение всех товаров.");

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении товара.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetById(id);
                if (product == null)
                {
                    _logger.LogWarning($"Товар с ID {id} не найден.");
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении товара с ID {id}.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateProductById")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                if (product == null)
                {
                    _logger.LogWarning("Попытка обновить товар с пустым телом запроса.");
                    return BadRequest("Товар не может быть пустым.");
                }

                var updatedProduct = await _productService.Update(id, product);
                if (updatedProduct == null)
                {
                    _logger.LogWarning($"Товар с ID {id} не найден.");
                    return NotFound();
                }

                _logger.LogInformation($"Товар с ID {id} был обновлен");
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении товара с ID {Id}.", id);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteProductById")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.Delete(id);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении товара с ID {id}.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }
    }
}
