namespace CareSync.Models
{
    public class FcmTokenUpdateModel
    {
        public string UserId { get; set; }
        public string? FcmToken { get; set; } // Make FcmToken nullable
    }
}
