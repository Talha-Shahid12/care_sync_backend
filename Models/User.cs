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
        public string? Token { get; set; }
        public string? FcmToken { get; set; }
        public ICollection<Image> Images { get; set; } // Navigation property
    }
    public class Image
    {
        public string? ImageId { get; set; }
        public string? UserId { get; set; }
        public byte[] ImageData { get; set; }
        public User User { get; set; } // Navigation property
    }
}
