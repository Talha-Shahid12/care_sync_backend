namespace CareSync.DTOs
{
    public class AppointmentDto
    {
        public string? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? Status { get; set; }
        public string? PatientId { get; set; }
        public string? DoctorId { get; set; }
    }
}
