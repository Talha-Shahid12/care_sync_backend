using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;
using CareSync.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;


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
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
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
            byte[]? imageData = null;

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

            var image = await _context.Images.FirstOrDefaultAsync(i => i.UserId == user.UserId);
            if (image != null)
            {
                imageData = image.ImageData;
            }

            return Ok(new
            {
                message = "Login successful!",
                token = user.Token,
                userId = user.UserId,
                userType = user.UserType,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                patientId = patientId,
                doctorId = doctorId,
                success = true,
                imageData = imageData != null ? Convert.ToBase64String(imageData) : "",
            });
        }

        [HttpGet("get-patient-info")]
        [Authorize]
        public async Task<IActionResult> GetPatientProfile([FromQuery] string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID is required." });
            }

            var user = await _context.Users
            .Join(_context.Patients,
                    u => u.UserId,         
                    p => p.UserId,         
                    (u, p) => new          
                {
                 u.UserId,          
                 u.FirstName,
                 u.LastName,
                 u.Email,
                 p.Dob,
                 p.ContactNumber
            })
            .Where(u => u.UserId == userId) 
            .FirstOrDefaultAsync();


            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { message = user, success = true });
        }

        [HttpPut("update-patient-info")]
        [Authorize]
   
        public async Task<IActionResult> UpdatePatientProfile([FromBody] UpdatePatientProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == dto.UserId);
            if (patient == null)
            {
                return NotFound(new { message = "Patient details not found" });
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            patient.Dob = dto.Dob;
            patient.ContactNumber = dto.ContactNumber;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully", success = true });
        }



        [HttpPut("update-doctor-info")]
        [Authorize]

        public async Task<IActionResult> UpdateDoctorProfile([FromBody] UpdateDoctorProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == dto.UserId);
            if (doctor == null)
            {
                return NotFound(new { message = "Patient details not found" });
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            doctor.Specialization = dto.Specialization;
            doctor.HospitalName = dto.HospitalName;
            doctor.ConsultationFee = dto.ConsultationFee;
            doctor.FreeHours = JsonConvert.SerializeObject(dto.FreeHours);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully", success = true });
        }



        [HttpGet("get-doctor-info")]
        [Authorize]
        public async Task<IActionResult> GetDoctorProfile([FromQuery] string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID is required." });
            }

            var user = await _context.Users
    .Join(_context.Doctors,
          u => u.UserId,
          d => d.UserId,
          (u, d) => new
          {
              u.UserId,
              u.FirstName,
              u.LastName,
              u.Email,
              d.Specialization,
              d.HospitalName,
              d.ConsultationFee,
              d.FreeHours
              //FreeHours = JsonConvert.SerializeObject(d.FreeHours) // This is correct
          })
    .Where(u => u.UserId == userId)
    .FirstOrDefaultAsync();



            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { message = user, success = true });
        }


        [HttpPost("update-fcm-token")]
        [Authorize]
        public async Task<IActionResult> UpdateFcmToken([FromBody] FcmTokenUpdateModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId))
                return BadRequest("UserId is missing");

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
           
                user.FcmToken = model.FcmToken;
           
           

            await _context.SaveChangesAsync();

            return Ok(new { message = "FCM token updated successfully", success = true });
        }



        [HttpGet("status")]
        public IActionResult CheckServerStatus()
        {
            return Ok("Server is running");
        }
    }
}
