using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                _logger.LogWarning("Попытка создать категорию с пустым названием.");
                return BadRequest("Название категории не может быть пустым.");
            }

            var createdCategory = await _categoryService.Create(category, cancellationToken);
            _logger.LogInformation($"Категория '{createdCategory.Name}' успешно создана.");

            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpGet("allCategories")]
        public IActionResult GetAllCategories(CancellationToken cancellationToken)
        {
            var categories = _categoryService.GetAll(cancellationToken);
            _logger.LogInformation("Запрос на получение всех категорий.");
            return Ok(categories);
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetById(id, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning($"Категория с ID {id} не найдена.");
                return NotFound();
            }

            return Ok(category);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category, CancellationToken cancellationToken)
        {
            if (category == null)
            {
                _logger.LogWarning("Попытка обновить категорию с пустым телом запроса.");
                return BadRequest("Категория не может быть пустой.");
            }

            var updatedCategory = await _categoryService.Update(id, category, cancellationToken);
            if (updatedCategory == null)
            {
                _logger.LogWarning($"Категория с ID {id} не найдена.");
                return NotFound();
            }

            _logger.LogInformation($"Категория с ID {id} была обновлена.");
            return Ok(updatedCategory);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var deletedCategory = await _categoryService.Delete(id, cancellationToken);
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
    }
}