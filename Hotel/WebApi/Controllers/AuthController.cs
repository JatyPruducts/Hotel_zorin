using Hotel.Domain.Interfaces;
using Hotel.Domain.Models.DTO;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.WebApi.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                // Доп. проверки при необходимости
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest("Username and password are required.");
                }

                var newUserId = await _userService.RegisterAsync(request.Username, request.Password);

                // Если метод бросает исключение при занятом логине – ловим в catch
                // или сами проверяем, что newUserId >= 1
                return Ok(new { UserId = newUserId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Register");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest("Username and password are required.");
                }

                var user = await _userService.LoginAsync(request.Username, request.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password.");
                }

                // В реальной ситуации выдали бы JWT
                // Пока просто возвращаем какие-то данные
                return Ok(new
                {
                    Id = user.Id,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
