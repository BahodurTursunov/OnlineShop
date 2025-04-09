using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Authorize(Roles = "1")]
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    _logger.LogWarning("Попытка создать категорию с пустым названием.");
                    return BadRequest("Название категории не может быть пустым.");
                }

                var createdCategory = await _categoryService.Create(category);
                _logger.LogInformation($"Категория '{createdCategory.Name}' успешно создана.");

                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании категории.");
                return StatusCode(500, $"Внутренняя ошибка сервера {ex}.");
            }
        }

        [HttpGet("GetAllCategories")]
        public IActionResult GetAllCategories()
        {
            try
            {
                var categories = _categoryService.GetAll();
                _logger.LogInformation("Запрос на получение всех категорий.");
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении категорий.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetById(id);
                if (category == null)
                {
                    _logger.LogWarning($"Категория с ID {id} не найдена.");
                    return NotFound();
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении категории с ID {id}.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategoryById")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            try
            {
                if (category == null)
                {
                    _logger.LogWarning("Попытка обновить категорию с пустым телом запроса.");
                    return BadRequest("Категория не может быть пустой.");
                }

                var updatedCategory = await _categoryService.Update(id, category);
                if (updatedCategory == null)
                {
                    _logger.LogWarning($"Категория с ID {id} не найдена.");
                    return NotFound();
                }

                _logger.LogInformation($"Категория с ID {id} была обновлена.");
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении категории с ID {id}.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategoryById")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var deletedCategory = await _categoryService.Delete(id);
                if (deletedCategory == null)
                {
                    _logger.LogWarning($"Категория с ID {id} не найдена при попытке удаления.");
                    return NotFound();
                }

                return Ok(new
                {
                    message = $"Категория '{deletedCategory.Name}' удалена.",
                    id = deletedCategory.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении категории с ID {id}.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }
    }
}
