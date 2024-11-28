using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using CareSync.Services; // Import the JWTService
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JWTService _jwtService; // Inject the JWTService

        public AuthController(IUserRepository userRepository, JWTService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService; // Initialize the JWTService
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email!);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists.");
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserType = registerDto.UserType,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            var token = _jwtService.GenerateToken(newUser.UserId, newUser.FirstName, newUser.LastName, newUser.UserType);
            newUser.Token = token;
            await _userRepository.AddUserAsync(newUser);

            return Ok(new { message = "Registration successful!" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email!);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            if (string.IsNullOrEmpty(user.Token))
            {
                return BadRequest(new { message = "User does not have a valid token." });
            }

            return Ok(new
            {
                message = "Login successful!",
                token = user.Token
            });
        }


        [HttpGet("status")]
        public IActionResult CheckServerStatus()
        {
            return Ok("Server is running");
        }
    }
}
