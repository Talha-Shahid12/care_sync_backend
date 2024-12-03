namespace CareSync.DTOs
{
    public class DoctorDetailsDto
    {
        public string? DoctorId { get; set; }
        public string? Specialization { get; set; }
        public string? HospitalName { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? DoctorName { get; set; }
        public List<FreeDayDto> FreeHours { get; set; }
    }
}

