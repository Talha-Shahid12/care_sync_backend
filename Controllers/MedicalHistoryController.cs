using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSync.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;
        private readonly AppDbContext _dbContext;

        public MedicalHistoryController(IMedicalHistoryRepository medicalHistoryRepository, AppDbContext context)
        {
            _dbContext = context;
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
            if (string.IsNullOrEmpty(medicalHistoryDto.Diagnosis) ||
                string.IsNullOrEmpty(medicalHistoryDto.Prescription))
            {
                return BadRequest("Diagnosis, and Prescription are required.");
            }

            var newMedicalHistory = new MedicalHistory
            {
                HistoryId = Guid.NewGuid().ToString(), 
                Diagnosis = medicalHistoryDto.Diagnosis,
                Prescription = medicalHistoryDto.Prescription,
                PatientId = medicalHistoryDto.PatientId,
                AppointmentId = medicalHistoryDto.AppointmentId,
            };

            await _medicalHistoryRepository.AddMedicalHistoryAsync(newMedicalHistory);

   
            var appointment = await _dbContext.Appointments
        .FirstOrDefaultAsync(a => a.AppointmentId == medicalHistoryDto.AppointmentId);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            appointment.Status = "COMPLETED";
            _dbContext.Appointments.Update(appointment);

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Medical history added successfully.", success = true });
        }
    }
}
