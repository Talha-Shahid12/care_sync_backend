using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using CareSync.DTOs;
namespace CareSync.Models
{
    public class Doctor
    {
        [Key]
        public string? DoctorId { get; set; }
        public string? Specialization { get; set; }
        public string? HospitalName { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? FreeHours { get; set; } // Store serialized JSON string
        [NotMapped]
        public List<FreeDayDto> FreeHoursList { get; set; } = new List<FreeDayDto>();  // Initialize to an empty list
        [ForeignKey("User")]
        public string UserId { get; set; }
    }
}
