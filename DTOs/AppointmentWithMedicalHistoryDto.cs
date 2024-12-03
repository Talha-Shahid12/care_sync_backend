namespace CareSync.DTOs
{
    public class AppointmentWithMedicalHistoryDto
    {
        public string? Status { get; set; }
        public string? AppointmentId { get; set; }
        public string? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? DoctorName { get; set; }
        public string? HospitalName { get; set; }
        public string? PatientId { get; set; }
        public List<MedicalHistoryDto> MedicalHistories { get; set; }
    }
  
}