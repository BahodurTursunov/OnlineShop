using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Exceptions;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    //[Authorize]
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

        //[Authorize(Roles = "Admin")]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("������� ������� ������������ � ������ ����� �������.");
                    return BadRequest("������������ �� ����� ���� ������.");
                }

                var createdUser = await _userService.Create(user);
                _logger.LogInformation($"������������ {createdUser.Username} ������� ������");

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (UsernameAlreadyExitstException)
            {
                return StatusCode(406, $"������������ � ������� {user.Username} ��� ����������.");

            }
            catch (UserMailAlreadyExistException)
            {
                return StatusCode(406, $"������������ � ������ {user.Email} ��� ����������.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� �������� ������������.");
                return StatusCode(500, "�� ��������� ��������� ������, ���� ������� � ����� ������ ��� ���������������");
            }
        }

        [HttpGet("GetAllUsers")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                var users = _userService.GetAll();
                _logger.LogInformation("������ �� ��������� ���� �������������.");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� ��������� �������������.");
                return StatusCode(500, "���������� ������ �������.");
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetById(id);
                if (user == null)
                {
                    _logger.LogWarning($"������������ � ID {id} �� ������.");
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"������ ��� ��������� ������������ � ID {id}.");
                return StatusCode(500, "���������� ������ �������.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUserById")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("������� �������� ������������ � ������ ����� �������.");
                    return BadRequest("������������ �� ����� ���� ������.");
                }

                var updatedUser = await _userService.Update(id, user);
                if (updatedUser == null)
                {
                    _logger.LogWarning($"������������ � ID {id} �� ������.");
                    return NotFound();
                }

                _logger.LogInformation($"������������ � ID {id} ��� ��������");
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� ���������� ������������ � ID {Id}.", id);
                return StatusCode(500, "���������� ������ �������.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUserById")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.Delete(id);
                if (result is null)
                {
                    _logger.LogWarning($"������������ � ID {id} �� ������ ��� ������� ��������.");
                    return NotFound();
                }

                return Ok(new
                {
                    message = $"������������ {result.Username} ������",
                    id = result.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"������ ��� �������� ������������ � ID {id}.");
                return StatusCode(500, "���������� ������ �������.");
            }
        }
    }
}
