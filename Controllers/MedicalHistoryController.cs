using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;

        public MedicalHistoryController(IMedicalHistoryRepository medicalHistoryRepository)
        {
            _medicalHistoryRepository = medicalHistoryRepository;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddMedicalHistory([FromBody] MedicalHistoryDto medicalHistoryDto)
        {
            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }
            if (string.IsNullOrEmpty(medicalHistoryDto.VisitDate) ||
                string.IsNullOrEmpty(medicalHistoryDto.Diagnosis) ||
                string.IsNullOrEmpty(medicalHistoryDto.Prescription))
            {
                return BadRequest("VisitDate, Diagnosis, and Prescription are required.");
            }

            var newMedicalHistory = new MedicalHistory
            {
                HistoryId = Guid.NewGuid().ToString(), 
                VisitDate = medicalHistoryDto.VisitDate, 
                Diagnosis = medicalHistoryDto.Diagnosis,
                Prescription = medicalHistoryDto.Prescription,
                PatientId = medicalHistoryDto.PatientId,
                AppointmentId = medicalHistoryDto.AppointmentId,
            };

            await _medicalHistoryRepository.AddMedicalHistoryAsync(newMedicalHistory);

            return Ok("Medical history added successfully.");
        }
    }
}
