using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly ILogger<CategoryController> _logger = logger;

        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                _logger.LogWarning("Attempt to create a category with an empty name.");
                return BadRequest("The category name cannot be empty..");
            }

            var createdCategory = await _categoryService.Create(category, cancellationToken);
            _logger.LogInformation($"Category '{createdCategory.Name}' successfully created.");

            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpGet("categories")]
        public IActionResult GetAllCategories(CancellationToken cancellationToken)
        {
            var categories = _categoryService.GetAll(cancellationToken);
            _logger.LogInformation("Request to retrieve all categories.");
            return Ok(categories);
        }

        [HttpGet("categories{id}")]
        public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetById(id, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} not found.");
                return NotFound();
            }

            return Ok(category);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("categories{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category, CancellationToken cancellationToken)
        {
            if (category == null)
            {
                _logger.LogWarning("Attempt to update a category with an empty request body.");
                return BadRequest("Category can not be empty.");
            }

            var updatedCategory = await _categoryService.Update(id, category, cancellationToken);
            if (updatedCategory == null)
            {
                _logger.LogWarning($"Category with ID {id} not found.");
                return NotFound();
            }

            _logger.LogInformation($"Category with ID {id} was update.");
            return Ok(updatedCategory);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("categories{id}")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var deletedCategory = await _categoryService.Delete(id, cancellationToken);
            
            if (deletedCategory == null)
            {
                _logger.LogWarning($"Category with ID {id} not found when attempting to delete.");
                return NotFound();
            }

            return Ok(new
            {
                message = $"Category '{deletedCategory.Name}' is deleted.",
                id = deletedCategory.Id
            });
        }
    }
}