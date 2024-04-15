using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeyGenerator.Data;
using KeyGenerator.Models;
using Microsoft.AspNetCore.Authorization;
using KeyGenerator.Services;
using System.Security.Claims;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CTypesController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        public CTypesController(KeyGeneratorDBContext context,ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CTypes
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CType>>> GetTypes()
        {
            return await _context.Types.ToListAsync();
        }

        // GET: api/CTypes/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CType>> GetCType(int id)
        {
            var cType = await _context.Types.FindAsync(id);

            if (cType == null)
            {
                return NotFound();
            }

            return cType;
        }

        // PUT: api/CTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCType(int id, CType cType)
        {
            if (id != cType.TypeID)
            {
                return BadRequest();
            }

            _context.Entry(cType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Updated to {cType.TypeName}", "CType", userId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error occurred while updating CType", "DbUpdateConcurrencyException", "CTypeController");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CType>> PostCType(CType cType)
        {
            _context.Types.Add(cType);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"CType: {cType.TypeName} Added ", "CType", userId);
            }


            return CreatedAtAction("GetCType", new { id = cType.TypeID }, cType);
        }

        // DELETE: api/CTypes/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCType(int id)
        {
            var cType = await _context.Types.FindAsync(id);
            if (cType == null)
            {
                return NotFound();
            }

            _context.Types.Remove(cType);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"CType: {cType.TypeName} Deleted ", "CType", userId);
            }

            return NoContent();
        }

        private bool CTypeExists(int id)
        {
            return _context.Types.Any(e => e.TypeID == id);
        }
    }
}
