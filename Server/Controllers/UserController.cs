using BaseLibrary.Entities;
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

        [HttpPost("Create user")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("Попытка создать пользователя с пустым телом запроса.");
                    return BadRequest("Пользователь не может быть пустым.");
                }

                var createdUser = await _userService.Create(user);
                _logger.LogInformation("Пользователь {Username} успешно создан", createdUser.Username);

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя.");
                return StatusCode(500, "Не корректно введенные данные, либо человек с такой почтой уже зарегистрирован");
            }
        }

        [HttpGet("Get all users")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                var users = _userService.GetAll();
                _logger.LogInformation("Запрос на получение всех пользователей.");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователей.");
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        [HttpGet("Get by id")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetById(id);
                if (user == null)
                {
                    _logger.LogWarning("Пользователь с ID {Id} не найден.", id);
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя с ID {Id}.", id);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // PUT: api/User/{id}
        [HttpPut("Update user by {id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("Попытка обновить пользователя с пустым телом запроса.");
                    return BadRequest("Пользователь не может быть пустым.");
                }

                var updatedUser = await _userService.Update(id, user);
                if (updatedUser == null)
                {
                    _logger.LogWarning("Пользователь с ID {Id} не найден.", id);
                    return NotFound();
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя с ID {Id}.", id);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // DELETE: api/User/{id}
        [HttpDelete("Delete user by {id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.Delete(id);
                if (result is null)
                {
                    _logger.LogWarning("Пользователь с ID {Id} не найден при попытке удаления.", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя с ID {Id}.", id);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }
    }
}
