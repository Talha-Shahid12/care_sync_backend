using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var userId = User.Identity.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }
            var newDoctor = new Doctor
            {
                DoctorId = Guid.NewGuid().ToString(),
                Specialization = doctorDto.Specialization,
                HospitalName = doctorDto.HospitalName,
                ConsultationFee = doctorDto.ConsultationFee,
                FreeHours = doctorDto.FreeHours,
                UserId = doctorDto.UserId
            };

            await _doctorRepository.AddDoctorAsync(newDoctor);

            return Ok("Doctor added successfully.");
        }
    }
}
