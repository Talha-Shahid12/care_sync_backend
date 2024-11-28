using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareSync.Models
{
    public class MedicalHistory
    {
        [Key]
        public string? HistoryId { get; set; }
        public string? VisitDate { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }

        [ForeignKey("Patient")]
        public string? PatientId { get; set; }
        public Patient? Patient { get; set; }

        [ForeignKey("Appointment")]
        public string? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
    }
}
