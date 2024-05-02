using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeyGenerator.Data;
using KeyGenerator.Models;
using KeyGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookletPdfDataController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        public BookletPdfDataController(KeyGeneratorDBContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/BookletPdfData
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookletPdfData>>> GetBookletPdfData()
        {
            return await _context.bookletPdfs.ToListAsync();
        }

        // GET: api/BookletPdfData/PaperID/5
        [Authorize]
        [HttpGet("PaperID/{paperID}")]
        public async Task<ActionResult<BookletPdfData>> GetBookletPdfDataByPaperID(int paperID)
        {
            var bookletPdfData = await _context.bookletPdfs.FirstOrDefaultAsync(pdf => pdf.PaperID == paperID);

            if (bookletPdfData == null)
            {
                return NotFound();
            }

            return bookletPdfData;
        }

        // PUT: api/BookletPdfData/PaperID/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("PaperID/{paperID}")]
        public async Task<IActionResult> PutBookletPdfDataByPaperID(int paperID, BookletPdfData bookletPdfData)
        {
            if (paperID != bookletPdfData.PaperID)
            {
                return BadRequest();
            }

            _context.Entry(bookletPdfData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Updated BookletPDFData with PaperID: {paperID}", "BookletPdfData", userId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookletPdfDataExists(paperID))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error occurred while updating BookletPDFData", "DbUpdateConcurrencyException", "BookletPdfDataController");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BookletPdfData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookletPdfData>> PostBookletPdfData(BookletPdfData bookletPdfData)
        {
            _context.bookletPdfs.Add(bookletPdfData);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"Added BookletPDFData with PaperID: {bookletPdfData.PaperID}", "BookletPdfData", userId);
            }

            return CreatedAtAction("GetBookletPdfDataByPaperID", new { paperID = bookletPdfData.PaperID }, bookletPdfData);
        }

        // DELETE: api/BookletPdfData/PaperID/5
        [Authorize]
        [HttpDelete("PaperID/{paperID}")]
        public async Task<IActionResult> DeleteBookletPdfDataByPaperID(int paperID)
        {
            var bookletPdfData = await _context.bookletPdfs.FirstOrDefaultAsync(pdf => pdf.PaperID == paperID);
            if (bookletPdfData == null)
            {
                return NotFound();
            }

            _context.bookletPdfs.Remove(bookletPdfData);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"Deleted BookletPDFData with PaperID: {paperID}", "BookletPdfData", userId);
            }

            return NoContent();
        }

        private bool BookletPdfDataExists(int paperID)
        {
            return _context.bookletPdfs.Any(e => e.PaperID == paperID);
        }
    }
}
