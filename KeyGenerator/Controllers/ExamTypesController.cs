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
    public class ExamTypesController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        public ExamTypesController(KeyGeneratorDBContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ExamTypes
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamType>>> GetExamTypes()
        {
            return await _context.ExamTypes.ToListAsync();
        }

        // GET: api/ExamTypes/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamType>> GetExamType(int id)
        {
            var examType = await _context.ExamTypes.FindAsync(id);

            if (examType == null)
            {
                return NotFound();
            }

            return examType;
        }

        // PUT: api/ExamTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamType(int id, ExamType examType)
        {
            if (id != examType.ExamTypeID)
            {
                return BadRequest();
            }

            _context.Entry(examType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogEvent($"Updated to {examType.ExamTypeName}", "ExamType", userId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error occurred while updating ExamType", "DbUpdateConcurrencyException", "ExamTypesController");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ExamTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ExamType>> PostExamType(ExamType examType)
        {
            _context.ExamTypes.Add(examType);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"ExamType: {examType.ExamTypeName} Added ", "ExamType", userId);
            }

            return CreatedAtAction("GetExamType", new { id = examType.ExamTypeID }, examType);
        }

        // DELETE: api/ExamTypes/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamType(int id)
        {
            var examType = await _context.ExamTypes.FindAsync(id);
            if (examType == null)
            {
                return NotFound();
            }

            _context.ExamTypes.Remove(examType);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogEvent($"ExamType: {examType.ExamTypeName} Deleted ", "ExamType", userId);
            }


            return NoContent();
        }

        private bool ExamTypeExists(int id)
        {
            return _context.ExamTypes.Any(e => e.ExamTypeID == id);
        }
    }
}
