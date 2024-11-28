using CareSync.Data;
using CareSync.Models;

namespace CareSync.Repositories
{
    public interface IPatientRepository
    {
        Task AddPatientAsync(Patient patient);
    }

    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPatientAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }
    }
}
