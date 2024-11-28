using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareSync.Models
{
    public class Doctor
    {
        [Key]
        public String? DoctorId { get; set; }
        public string? Specialization { get; set; }
        public string? HospitalName { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? FreeHours { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }

    }
}
