using Microsoft.EntityFrameworkCore;
using CareSync.Models;

namespace CareSync.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<MedicalHistory> MedicalHistories { get; set;} = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;

    }
}
