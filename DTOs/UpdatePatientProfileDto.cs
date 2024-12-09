namespace CareSync.DTOs
{

    public class UpdatePatientProfileDto
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Dob { get; set; }
    }

}