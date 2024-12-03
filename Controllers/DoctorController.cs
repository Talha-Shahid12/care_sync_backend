using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorDto doctorDto)
        {
            if (doctorDto == null)
            {
                return BadRequest("Doctor details are required.");
            }

            if (doctorDto.FreeHours == null || !doctorDto.FreeHours.Any())
            {
                return BadRequest("FreeHours cannot be empty.");
            }

            var newDoctor = new Doctor
            {
                DoctorId = Guid.NewGuid().ToString(),
                Specialization = doctorDto.Specialization,
                HospitalName = doctorDto.HospitalName,
                ConsultationFee = doctorDto.ConsultationFee,
                FreeHours = JsonConvert.SerializeObject(doctorDto.FreeHours),
                UserId = doctorDto.UserId
            };

            await _doctorRepository.AddDoctorAsync(newDoctor);

            return Ok(new { message = "Doctor added successfully." , success = true});
        }



        [HttpGet("get-doctors")]
        [Authorize]
        public async Task<IActionResult> GetDoctors()
        {
            var userId = User.Identity.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }
            var doctors = await _doctorRepository.GetDoctorsWithNameAsync();
            if (doctors == null || !doctors.Any())
            {
                return NotFound(new { message = "No doctors found.", success = false });
            }
            return Ok(new { message = doctors, success = true });
        }
    }
}
