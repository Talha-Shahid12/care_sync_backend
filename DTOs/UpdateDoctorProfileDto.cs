namespace CareSync.DTOs
{
    public class UpdateDoctorProfileDto
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Specialization { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? HospitalName { get; set; }
        public List<FreeDayDto> FreeHours { get; set; }

    }

    
}