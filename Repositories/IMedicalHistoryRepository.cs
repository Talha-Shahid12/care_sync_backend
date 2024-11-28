using CareSync.Data;
using CareSync.Models;

namespace CareSync.Repositories
{
    public interface IMedicalHistoryRepository
    {
        Task AddMedicalHistoryAsync(MedicalHistory medicalHistory);
    }
    public class MedicalHistoryRepository : IMedicalHistoryRepository
    {
        private readonly AppDbContext _context;

        public MedicalHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddMedicalHistoryAsync(MedicalHistory medicalHistory)
        {
            await _context.MedicalHistories.AddAsync(medicalHistory);
            await _context.SaveChangesAsync();
        }
    }
}
