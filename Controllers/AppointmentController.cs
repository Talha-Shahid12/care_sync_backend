using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSync.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly AppDbContext _context;

        public AppointmentController(IAppointmentRepository appointmentRepository, AppDbContext context)
        {
            _context = context;
            _appointmentRepository = appointmentRepository;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDto appointmentDto)
        {
            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token. User ID not found." , success = false});
            }
            if (string.IsNullOrEmpty(appointmentDto.AppointmentDate) ||
                string.IsNullOrEmpty(appointmentDto.AppointmentTime) ||
                string.IsNullOrEmpty(appointmentDto.PatientId) ||
                string.IsNullOrEmpty(appointmentDto.DoctorId))
            {
                return BadRequest(new { message = "AppointmentDate, AppointmentTime, PatientId, and DoctorId are required." , success = false});
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

            return Ok(new { message = "Appointment added successfully.", success = true });
        }


        [HttpGet("get-appointments")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentsWithMedicalHistory([FromQuery] string patientId)
        {
            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token. User ID not found.", success = false });
            }
            if (string.IsNullOrEmpty(patientId))
            {
                Console.WriteLine("Came");
                return BadRequest(new { message = "PatientId is required.", success = false });
            }

            Console.WriteLine("Came");

            var appointmentsWithDetails = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Join(_context.Doctors, a => a.DoctorId, d => d.DoctorId, (a, d) => new
                {
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    DoctorId = a.DoctorId,
                    HospitalName = d.HospitalName,
                    UserId = d.UserId,
                    PatientId = a.PatientId
                })
                .Join(_context.Users, ad => ad.UserId, u => u.UserId, (ad, u) => new
                {
                    ad.AppointmentId,
                    ad.AppointmentDate,
                    ad.AppointmentTime,
                    ad.Status,
                    DoctorName = u.FirstName + " " + u.LastName,
                    ad.HospitalName,
                    ad.PatientId
                })
                .ToListAsync();

            var result = new List<AppointmentWithMedicalHistoryDto>();

            foreach (var appointment in appointmentsWithDetails)
            {
                var medicalHistories = await _context.MedicalHistories
                    .Where(mh => mh.PatientId == appointment.PatientId && mh.AppointmentId == appointment.AppointmentId)
                    .Select(mh => new MedicalHistoryDto
                    {
                        Diagnosis = mh.Diagnosis,
                        Prescription = mh.Prescription,
                        PatientId = mh.PatientId,
                        AppointmentId = mh.AppointmentId
                    })
                    .ToListAsync();

                result.Add(new AppointmentWithMedicalHistoryDto
                {
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    Status = appointment.Status,
                    DoctorName = appointment.DoctorName,
                    HospitalName = appointment.HospitalName,
                    PatientId = appointment.PatientId,
                    MedicalHistories = medicalHistories
                });
            }

         

            return Ok(new { message = result, success = true });
        }



        [HttpGet("get-appointments-for-doctor")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentsForDoctor([FromQuery] string doctorId)
        {

            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token. User ID not found.", success = false });
            }
            if (string.IsNullOrEmpty(doctorId))
            {
                return BadRequest(new { message = "DoctorId is required.", success = false });
            }

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == "SCHEDULED")
                .Join(_context.Patients, a => a.PatientId, p => p.PatientId, (a, p) => new
                {
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    p.Dob,
                    p.ContactNumber,
                    p.UserId,
                    a.PatientId
                })
                .Join(_context.Users, ap => ap.UserId, u => u.UserId, (ap, u) => new
                {
                    ap.AppointmentId,
                    ap.AppointmentDate,
                    ap.AppointmentTime,
                    ap.Status,
                    PatientName = u.FirstName + " " + u.LastName,
                    ap.Dob,
                    ap.ContactNumber,
                    ap.PatientId
                })
                .ToListAsync();

       

            var result = new List<object>();

            foreach (var appointment in appointments)
            {
                var medicalHistories = await _context.MedicalHistories
                    .Where(mh => mh.PatientId == appointment.PatientId && mh.AppointmentId == appointment.AppointmentId)
                    .Select(mh => new
                    {
                        mh.Diagnosis,
                        mh.Prescription
                    })
                    .ToListAsync();

                result.Add(new
                {
                    appointment.AppointmentId,
                    appointment.AppointmentDate,
                    appointment.AppointmentTime,
                    appointment.Status,
                    appointment.PatientName,
                    appointment.Dob,
                    appointment.ContactNumber,
                    appointment.PatientId,
                    MedicalHistories = medicalHistories
                });
            }

            return Ok(new { message = result, success = true });
        }

        [HttpGet("get-all-appointments-for-doctor")]
        [Authorize]
        public async Task<IActionResult> GetAllAppointmentsForDoctor([FromQuery] string doctorId)
        {

            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token. User ID not found.", success = false });
            }
            if (string.IsNullOrEmpty(doctorId))
            {
                return BadRequest(new { message = "DoctorId is required.", success = false });
            }

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Join(_context.Patients, a => a.PatientId, p => p.PatientId, (a, p) => new
                {
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    p.Dob,
                    p.ContactNumber,
                    p.UserId,
                    a.PatientId
                })
                .Join(_context.Users, ap => ap.UserId, u => u.UserId, (ap, u) => new
                {
                    ap.AppointmentId,
                    ap.AppointmentDate,
                    ap.AppointmentTime,
                    ap.Status,
                    PatientName = u.FirstName + " " + u.LastName,
                    ap.Dob,
                    ap.ContactNumber,
                    ap.PatientId
                })
                .ToListAsync();


            var result = new List<object>();

            foreach (var appointment in appointments)
            {
                var medicalHistories = await _context.MedicalHistories
                    .Where(mh => mh.PatientId == appointment.PatientId && mh.AppointmentId == appointment.AppointmentId)
                    .Select(mh => new
                    {
                        mh.Diagnosis,
                        mh.Prescription
                    })
                    .ToListAsync();

                result.Add(new
                {
                    appointment.AppointmentId,
                    appointment.AppointmentDate,
                    appointment.AppointmentTime,
                    appointment.Status,
                    appointment.PatientName,
                    appointment.Dob,
                    appointment.ContactNumber,
                    appointment.PatientId,
                    MedicalHistories = medicalHistories
                });
            }

            return Ok(new { message = result, success = true });
        }


    }
}
