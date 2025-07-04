﻿using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ServerLibrary.Services.Contracts.Auth;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Registration
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto, CancellationToken cancellationToken) // предназначен для регистрации пользователей
        {
            var result = await _authService.RegisterAsync(dto, cancellationToken);
            return Ok(result); // Можно вернуть JWT сразу, если нужно авто-вход
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(new { token });
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto, CancellationToken cancellationToken)
        //{
        //    var response = await _authService.RefreshTokenAsync(dto, cancellationToken);
        //    return Ok(response);
        //}

        /* [Authorize]
         [HttpGet("profile")]
         public async Task<IActionResult> GetProfile([FromBody] string id, CancellationToken cancellationToken)
         {
             var userId = int.Parse(User.FindFirst(id)?.Value ?? throw new UnauthorizedAccessException());
             var profile = await _authService.GetUserProfileAsync(userId, cancellationToken);
             return Ok(profile);
         }*/
    }
}
