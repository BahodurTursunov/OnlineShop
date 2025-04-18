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
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user, CancellationToken cancellationToken)
        {

            if (user == null)
            {
                _logger.LogWarning("Попытка создать пользователя с пустым телом запроса.");
                return BadRequest("Пользователь не может быть пустым.");
            }

            var createdUser = await _userService.Create(user, cancellationToken);
            _logger.LogInformation($"Пользователь {createdUser.Username} успешно создан");

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = _userService.GetAll(cancellationToken);
            _logger.LogInformation("Запрос на получение всех пользователей.");

            return Ok(users);
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetById(id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь с ID {id} не найден.");
                return NotFound();
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
                _logger.LogWarning("Попытка обновить пользователя с пустым телом запроса.");
                return BadRequest("Пользователь не может быть пустым.");
            }

            var updatedUser = await _userService.Update(id, user, cancellationToken);
            if (updatedUser == null)
            {
                _logger.LogWarning($"Пользователь с ID {id} не найден.");
                return NotFound();
            }

            _logger.LogInformation($"Пользователь с ID {id} был обновлен");
            return Ok(updatedUser);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.Delete(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning($"Пользователь с ID {id} не найден при попытке удаления.");
                return NotFound();
            }

            return Ok(new
            {
                message = $"Пользователь {result.Username} удален",
                id = result.Id
            });
        }
    }
}
