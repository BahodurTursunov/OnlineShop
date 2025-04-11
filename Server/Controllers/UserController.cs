using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {

            if (user == null)
            {
                _logger.LogWarning("Попытка создать пользователя с пустым телом запроса.");
                return BadRequest("Пользователь не может быть пустым.");
            }

            var createdUser = await _userService.Create(user);
            _logger.LogInformation($"Пользователь {createdUser.Username} успешно создан");

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userService.GetAll();
            _logger.LogInformation("Запрос на получение всех пользователей.");

            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь с ID {id} не найден.");
                return NotFound();
            }
            return Ok(user);
        }

        [Authorize/*(Roles = "Admin")*/]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Попытка обновить пользователя с пустым телом запроса.");
                return BadRequest("Пользователь не может быть пустым.");
            }

            var updatedUser = await _userService.Update(id, user);
            if (updatedUser == null)
            {
                _logger.LogWarning($"Пользователь с ID {id} не найден.");
                return NotFound();
            }

            _logger.LogInformation($"Пользователь с ID {id} был обновлен");
            return Ok(updatedUser);
        }

        [Authorize/*(Roles = "Admin")*/]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.Delete(id);
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
