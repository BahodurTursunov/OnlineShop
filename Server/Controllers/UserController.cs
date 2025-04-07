using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Exceptions;
using ServerLibrary.Services.Contracts;

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
        if (user == null)
        {
            _logger.LogWarning("������� ������� ������������ � ������ ����� �������.");
            return BadRequest(new { message = "������������ �� ����� ���� ������." });
        }

        try
        {
            var createdUser = await _userService.Create(user);
            _logger.LogInformation($"������������ {user.Username} ������� ������");

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, new { status = "success", data = createdUser });
        }
        catch (UsernameAlreadyExitstException ex)
        {
            _logger.LogError(ex, $"������: {ex.Message}");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������ ��� �������� ������������.");
            return StatusCode(500, new { message = "��������� ������ �� �������" });
        }
    }

    [HttpGet("Get all")]
    public async Task<IActionResult> GetAll()
    {
        var user = _userService.GetAll();
        return Ok(user);
    }

    [HttpGet("Get by id")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                _logger.LogWarning("������������ � ID {Id} �� ������.", id);
                return NotFound(new { message = "������������ �� ������" });
            }

            return Ok(new { status = "success", data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������ ��� ��������� ������������ � ID {Id}.", id);
            return StatusCode(500, new { message = "��������� ������ �� �������" });
        }
    }
    // PUT: api/User/{id}
    [HttpPut("Update user by id")]
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
                _logger.LogWarning("������������ � ID {Id} �� ������.", id);
                return NotFound();
            }

            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������ ��� ���������� ������������ � ID {Id}.", id);
            return StatusCode(500, "���������� ������ �������.");
        }
    }

    // DELETE: api/User/{id}
    [HttpDelete("Delete user by id")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.Delete(id);
            if (result is null)
            {
                _logger.LogWarning("������������ � ID {Id} �� ������ ��� ������� ��������.", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������ ��� �������� ������������ � ID {Id}.", id);
            return StatusCode(500, "���������� ������ �������.");
        }
    }
}

