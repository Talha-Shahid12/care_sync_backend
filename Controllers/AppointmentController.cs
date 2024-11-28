using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDto appointmentDto)
        {
            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }
            if (string.IsNullOrEmpty(appointmentDto.AppointmentDate) ||
                string.IsNullOrEmpty(appointmentDto.AppointmentTime) ||
                string.IsNullOrEmpty(appointmentDto.PatientId) ||
                string.IsNullOrEmpty(appointmentDto.DoctorId))
            {
                return BadRequest("AppointmentDate, AppointmentTime, PatientId, and DoctorId are required.");
            }

            var newAppointment = new Appointment
            {
                AppointmentId = Guid.NewGuid().ToString(),
                AppointmentDate = appointmentDto.AppointmentDate,
                AppointmentTime = appointmentDto.AppointmentTime,
                Status = appointmentDto.Status,
                PatientId = appointmentDto.PatientId,
                DoctorId = appointmentDto.DoctorId
            };

            await _appointmentRepository.AddAppointmentAsync(newAppointment);

            return Ok("Appointment added successfully.");
        }
    }
}
