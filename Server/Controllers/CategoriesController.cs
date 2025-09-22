using AutoMapper;
using BaseLibrary.DTOs;
using BaseLibrary.DTOs.Cart;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger, IMapper mapper) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CategoriesController> _logger = logger;

        /// <summary>
        /// Create category
        /// </summary>
        /// <param name="createCategoryDTO"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO createCategoryDTO, CancellationToken cancellationToken)
        {
           var categoryToCreate = _mapper.Map<Category>(createCategoryDTO);
            try
            {
                var createdCategory = await _categoryService.Create(categoryToCreate, cancellationToken);
                var categoryDTO = _mapper.Map<CategoryDTO>(createdCategory);

                return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDTO.Id }, categoryDTO);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Conflict during category creation: {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAll(cancellationToken).ToListAsync(cancellationToken);

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return Ok(categoryDtos);
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
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.GetById(id, cancellationToken);

                var categoryDto = _mapper.Map<CategoryDTO>(category);

                return Ok(categoryDto);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Category with ID {Id} not found.", id);
                return NotFound();
            }

        }

        /// <summary>
        /// Update category by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDTO updateDto, CancellationToken cancellationToken)
        {
            try
            {
                var categoryToUpdate = _mapper.Map<Category>(updateDto);
                await _categoryService.Update(id, categoryToUpdate, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
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
            try
            {
                await _categoryService.Delete(id, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NoContent();
            }
        }
    }
}