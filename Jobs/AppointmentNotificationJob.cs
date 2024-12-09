using CareSync.Data;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace CareSync.Jobs
{
    public class AppointmentNotificationJob : IJob
    {
        private readonly AppDbContext _dbContext;

        public AppointmentNotificationJob(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var nextDayString = nextDay.ToString("dd-MM-yyyy");

            var appointments = await _dbContext.Appointments
                .Where(a => a.AppointmentDate == nextDayString && a.Status == "SCHEDULED")
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                if (appointment.PatientId != null && !string.IsNullOrEmpty(appointment.PatientId))
                {
                    var patient = await _dbContext.Patients
                        .FirstOrDefaultAsync(p => p.PatientId == appointment.PatientId);

                    if (patient != null)
                    {
                        var user = await _dbContext.Users
                            .FirstOrDefaultAsync(u => u.UserId == patient.UserId);

                        if (user != null && !string.IsNullOrEmpty(user.FcmToken))
                        {
                            await SendNotification(
                            user.FcmToken,
                            "Soft Reminder",
                            $"You Have An Appointment on {appointment.AppointmentDate} at {appointment.AppointmentTime}"
                            );


                        }
                        else
                        {
                            Console.WriteLine($"No FCM token found for user {patient.UserId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Patient not found for PatientId {appointment.PatientId}");
                    }
                }
            }




        }

        private async Task SendNotification(string fcmToken, string title, string body)
        {
            var message = new Message()
            {
                Token = fcmToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
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
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Notification sent successfully " + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending notification: " + ex.Message);
            }
        }
    }
}
