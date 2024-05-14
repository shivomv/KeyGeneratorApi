using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeyGenerator.Data;
using KeyGenerator.Models;
using KeyGenerator.Services;
using System.Security.Claims;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterKeyFileController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        public MasterKeyFileController(KeyGeneratorDBContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/MasterKeyFile
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MasterKeyFile>>> GetMasterKeyFiles()
        {
            return await _context.masterKeyFiles.ToListAsync();
        }

        // GET: api/MasterKeyFile/PaperID/5
        [Authorize]
        [HttpGet("PaperID/{paperID}")]
        public async Task<ActionResult<MasterKeyFile>> GetMasterKeyFileByPaperID(int paperID)
        {
            var masterKeyFile = await _context.masterKeyFiles.FirstOrDefaultAsync(file => file.PaperID == paperID);

            if (masterKeyFile == null)
            {
                return NotFound();
            }

            return masterKeyFile;
        }

        // PUT: api/MasterKeyFile/PaperID/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("PaperID/{paperID}")]
        public async Task<IActionResult> PutMasterKeyFileByPaperID(int paperID, MasterKeyFile masterKeyFile)
        {
            if (paperID != masterKeyFile.PaperID)
            {
                return BadRequest();
            }

            _context.Entry(masterKeyFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogEvent($"Updated MasterKeyFile in PaperID: {paperID}", "MasterKeyFile", userId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MasterKeyFileExists(paperID))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error occurred while updating MasterKeyFile", "DbUpdateConcurrencyException", "MasterKeyFileController");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MasterKeyFile
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MasterKeyFile>> PostMasterKeyFile(MasterKeyFile masterKeyFile)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                masterKeyFile.UploadedBy = userId;
            }
            else
            {
                return Unauthorized();
            }

            masterKeyFile.UploadedDateTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            _context.masterKeyFiles.Add(masterKeyFile);
            await _context.SaveChangesAsync();

            _logger.LogEvent($"Added MasterKeyFile in PaperID: {masterKeyFile.PaperID}", "MasterKeyFile", userId);

            return CreatedAtAction("GetMasterKeyFileByPaperID", new { paperID = masterKeyFile.PaperID }, masterKeyFile);
        }

        // DELETE: api/MasterKeyFile/PaperID/5
        [Authorize]
        [HttpDelete("PaperID/{paperID}")]
        public async Task<IActionResult> DeleteMasterKeyFileByPaperID(int paperID)
        {
            var masterKeyFile = await _context.masterKeyFiles.FirstOrDefaultAsync(file => file.PaperID == paperID);
            if (masterKeyFile == null)
            {
                return NotFound();
            }

            _context.masterKeyFiles.Remove(masterKeyFile);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogEvent($"Deleted MasterKeyFile in PaperID: {paperID}", "MasterKeyFile", userId);
            }

            return NoContent();
        }

        private bool MasterKeyFileExists(int paperID)
        {
            return _context.masterKeyFiles.Any(e => e.PaperID == paperID);
        }
    }
}
