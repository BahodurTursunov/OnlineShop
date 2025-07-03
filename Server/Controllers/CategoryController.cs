using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly ILogger<CategoryController> _logger = logger;

        /// <summary>
        /// Create category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("categories")]
        public IActionResult GetAllCategories(CancellationToken cancellationToken)
        {
            var categories = _categoryService.GetAll(cancellationToken);
            _logger.LogInformation("Request to retrieve all categories.");
            return Ok(categories);
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("categories/{id}")]
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

        /// <summary>
        /// Update category by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpPut("categories/{id}")]
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

        /// <summary>
        /// Delete category by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpDelete("categories/{id}")]
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