using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ILogger<UserController> _logger = logger;

        /// <summary>
        /// Create Admin
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] User user, CancellationToken cancellationToken) // предназначен для создания Админов системы
        {
            if (user == null)
            {
                _logger.LogWarning("Attempt to create a user with an empty request body.");
                return BadRequest("User cannot be empty.");
            }

            var createdUser = await _userService.Create(user, cancellationToken);
            _logger.LogInformation($"User {createdUser.Username} successfully created.");

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("users")]
        public ActionResult<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = _userService.GetAll(cancellationToken);
            _logger.LogInformation("Request to retrieve all users.");

            return Ok(users);
        }

        /// <summary>
        /// Get User by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetById(id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Update User by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempt to update a user with an empty request body.");
                return BadRequest("User cannot be empty.");
            }

            var updatedUser = await _userService.Update(id, user, cancellationToken);

            if (updatedUser == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound();
            }

            _logger.LogInformation($"User with ID {id} was updated.");
            return Ok(updatedUser);
        }


        /// <summary>
        /// Delete user by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.Delete(id, cancellationToken);

            if (result is null)
            {
                _logger.LogWarning($"User with ID {id} not found when attempting to delete.");
                return NotFound();
            }

            return Ok(new
            {
                message = $"User {result.Username} deleted.",
                id = result.Id
            });
        }
    }
}