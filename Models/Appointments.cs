using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareSync.Models
{
    public class Appointment
    {
        [Key]
        public string? AppointmentId { get; set; }

        public string? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? Status { get; set; }

        [ForeignKey("Patient")]
        public string? PatientId { get; set; } 
        public Patient? Patient { get; set; }  

        [ForeignKey("Doctor")]
        public string? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }    
    }
}
