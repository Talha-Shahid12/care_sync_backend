using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddPatient([FromBody] PatientDto patientDto)
        {
            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }
            var newPatient = new Patient
            {
                PatientId = Guid.NewGuid().ToString(),
                Dob = patientDto.Dob,
                ContactNumber = patientDto.ContactNumber,
                UserId = patientDto.UserId
            };

            await _patientRepository.AddPatientAsync(newPatient);

            return Ok("Patient added successfully.");
        }
    }
}
