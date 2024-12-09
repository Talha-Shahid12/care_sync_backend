using Microsoft.AspNetCore.Mvc;
using CareSync.Data;
using CareSync.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImagesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromBody] ImageUploadModel model)
        {
            Console.WriteLine("Came in ......");

            if (model == null || string.IsNullOrEmpty(model.Base64Image))
                return BadRequest("No image data provided");

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
                return NotFound("User not found");

            byte[] imageData;
            try
            {
                imageData = Convert.FromBase64String(model.Base64Image);
            }
            catch (FormatException ex)
            {
                return BadRequest(new { message = "Invalid base64 string", Error = ex.Message, success = false });
            }

            var existingImage = await _context.Images.FirstOrDefaultAsync(i => i.UserId == model.UserId);

            if (existingImage != null)
            {
                existingImage.ImageData = imageData;
            }
            else
            {
                var newImage = new Image
                {
                    ImageId = Guid.NewGuid().ToString(),
                    UserId = model.UserId,
                    ImageData = imageData
                };
                _context.Images.Add(newImage);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Image uploaded successfully", success = true });
        }



        [HttpGet("get-image/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetImageByUserId(string userId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(e => e.UserId == userId);

            if (image == null)
            {
                return NotFound(new { message = "Image not found", UserId = userId, success = false });
            }

            var imageBase64 = Convert.ToBase64String(image.ImageData);

            return Ok(new
            {
                message = "Image retrieved successfully",
                success = true,
                imageId = image.ImageId,
                imageData = imageBase64
            });
        }

    }
}
