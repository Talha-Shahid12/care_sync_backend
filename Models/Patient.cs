using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareSync.Models
{
    public class Patient
    {
        [Key]
        public String? PatientId { get; set; }
        public string? Dob { get; set; }
        public string? ContactNumber { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
