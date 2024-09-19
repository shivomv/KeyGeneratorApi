using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using KeyGenerator.Services;
using System.Security.Claims;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILoggerService _logger;

        public PdfUploadController(IWebHostEnvironment environment, ILoggerService logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected for upload.");
            }

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            // Ensure the file is saved in the wwwroot/uploads directory
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name to prevent overwriting
            string uniqueFileName = file.FileName;

            // Combine the path with the file name
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogEvent($"Uploaded PDF file: {file.FileName}", "PdfUpload", userId);
                }

                return Ok(new { FilePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while uploading PDF", ex.Message, "PdfUploadController");
                return StatusCode(500, "Internal server error while uploading the file.");
            }
        }
    }
}
