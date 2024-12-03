using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;
using CareSync.Data;
using Microsoft.EntityFrameworkCore;


namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JWTService _jwtService;
        private readonly AppDbContext _context;


        public AuthController(IUserRepository userRepository, JWTService jwtService, AppDbContext context)
        {
            _context = context;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email!);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists.", success = false });
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

            return Ok(new { message = "Registration successful!", success = true, userId = newUser.UserId, token = newUser.Token });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email!);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            string? patientId = null;
            string? doctorId = null;
          
            if (_context == null)
            {
                return StatusCode(500, new { message = "Server error: Database context not available." });
            }

            if (user.UserType == "PATIENT")
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.UserId);
                patientId = patient?.PatientId;
            }
            else if (user.UserType == "DOCTOR")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.UserId);
                doctorId = doctor?.DoctorId;
            }

            return Ok(new
            {
                message = "Login successful!",
                token = user.Token,
                userId = user.UserId,
                userType = user.UserType,
                patientId = patientId,
                doctorId = doctorId,
                success = true
            });
        }



        [HttpGet("status")]
        public IActionResult CheckServerStatus()
        {
            return Ok("Server is running");
        }
    }
}
