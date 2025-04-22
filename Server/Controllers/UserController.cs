using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user, CancellationToken cancellationToken)
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

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = _userService.GetAll(cancellationToken);
            _logger.LogInformation("Request to retrieve all users.");

            return Ok(users);
        }

        [HttpGet("getById")]
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

        [Authorize(Roles = "Admin")]
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("update")]
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
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

