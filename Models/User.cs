using System.ComponentModel.DataAnnotations;

namespace CareSync.Models
{
    public class User
    {
        [Key]
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserType { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; } // Add this property
    }
}
