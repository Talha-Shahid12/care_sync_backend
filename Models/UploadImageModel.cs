using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareSync.Models
{
    public class ImageUploadModel
    {
        public string? ImageId { get; set; }
        public string? UserId { get; set; }
        public string? Base64Image { get; set; }
    }
}