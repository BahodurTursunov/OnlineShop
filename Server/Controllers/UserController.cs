using AutoMapper;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Exceptions;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserController> _logger = logger;

        /// <summary>
        /// Create Admin
        /// </summary>
        /// <param name="createUserDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpPost("users")]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] CreateUserDTO createUserDto, CancellationToken cancellationToken) // предназначен для создания Админов системы
        {
          
            var userToCreate = _mapper.Map<User>(createUserDto);

            try
            {
                var createdUser = await _userService.Create(userToCreate, cancellationToken);

                var userDto = _mapper.Map<UserDTO>(createdUser);

                return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
            }
            catch (UsernameAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, $"Username {createUserDto.Username} already exist");
                return Conflict(new { message = ex.Message });
            }
            catch (UserMailAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, $"Email {createUserDto.Email} already exist");
                return Conflict(new { message = ex.Message });
            }
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
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAll(cancellationToken).ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(userDtos);
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
            try
            {
                var user = await _userService.GetById(id, cancellationToken);

                var userDto = _mapper.Map<UserDTO>(user);

                return Ok(userDto);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound();
            }
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
            try
            {
                var userToUpdate = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                await _userService.Update(id, userToUpdate, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
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
            try
            {
                await _userService.Delete(id, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NoContent();
            }
        }
    }
}