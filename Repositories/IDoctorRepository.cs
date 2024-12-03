using Microsoft.EntityFrameworkCore;
using CareSync.Models;
using CareSync.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareSync.DTOs;
using Newtonsoft.Json;

namespace CareSync.Repositories
{
    public interface IDoctorRepository
    {
        Task AddDoctorAsync(Doctor doctor);
        Task<IEnumerable<DoctorDetailsDto>> GetDoctorsWithNameAsync();
    }

    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
            Console.WriteLine(JsonConvert.SerializeObject(doctor)); 
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DoctorDetailsDto>> GetDoctorsWithNameAsync()
        {
            var doctors = await _context.Doctors
                .Join(_context.Users,
                    doctor => doctor.UserId,
                    user => user.UserId,
                    (doctor, user) => new DoctorDetailsDto
                    {
                        DoctorId = doctor.DoctorId,
                        Specialization = doctor.Specialization,
                        HospitalName = doctor.HospitalName,
                        ConsultationFee = doctor.ConsultationFee,
                        DoctorName = "Dr. " + user.FirstName + " " + user.LastName,
                        FreeHours = doctor.FreeHours != null
                                    ? JsonConvert.DeserializeObject<List<FreeDayDto>>(doctor.FreeHours)
                                    : new List<FreeDayDto>()  
                    })
                .ToListAsync();

            return doctors;
        }







    }
}
