using CareSync.Data;
using CareSync.Models;

namespace CareSync.Repositories
{
    public interface IAppointmentRepository
    {
        Task AddAppointmentAsync(Appointment appointment);
    }
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
