using CareSync.Data;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public NotificationsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendNotifications()
        {
            // Fetch all appointments (no filtering by date)
            var appointments = await _dbContext.Appointments
                .Where(a => a.Patient != null && !string.IsNullOrEmpty(a.PatientId)) // Ensure there's a valid patient and PatientId
                .ToListAsync();

            // Loop through all appointments and send notifications
            foreach (var appointment in appointments)
            {
                await SendNotification(
                    appointment.PatientId,
                    "Upcoming Appointment",
                    $"You have an appointment on {appointment.AppointmentDate} at {appointment.AppointmentTime}."
                );
            }

            return Ok("Notifications sent successfully.");
        }

        private async Task SendNotification(string patientId, string title, string body)
        {
            var message = new Message()
            {
                Token = "dbaMgAvgTnKgFgjH7fqnX-:APA91bEzHSA57Kck0U5X14pahaaA85jKfBJKQsRKEv5g4PokmWK2QhdQS9WEcsZNOb9xMRgfbwlnZozDFOcb9_ZxuS8zriCgRCwD-Nvt_jie55m8WTF5Hxg", // Use the actual patient FCM token here
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default",
                        Tag = "appointment"
                    }
                }
            };

            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Notification sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending notification: " + ex.Message);
            }
        }
    }
}
