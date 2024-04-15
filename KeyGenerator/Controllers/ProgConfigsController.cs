using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeyGenerator.Data;
using KeyGenerator.Models;
using KeyGenerator.Models.NonDBModels;
using KeyGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Org.BouncyCastle.Utilities;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgConfigsController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        public ProgConfigsController(KeyGeneratorDBContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ProgConfigs
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgConfig>>> GetProgConfigs()
        {
            return await _context.ProgConfigs.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProgConfigbyId(int id)
        {
            var progconfig = await _context.ProgConfigs.FindAsync(id);

            var steps = await _context.SetofSteps
    .Where(s => s.ProgConfigID == progconfig.ProgConfigID)
    .Select(s => s.steps) // Assuming 'steps' is the name of the column
    .ToListAsync();

            return Ok(new { Steps = steps });

        }

        // GET: api/ProgConfigs/5
        [Authorize]
        [HttpGet("Programme/{id}/{bookletsize}")]
        public async Task<ActionResult<IQueryable<ProgConfig>>> GetProgConfigbyProgID(int id,int bookletsize )
        {
            var progConfig = _context.ProgConfigs.Where(obj => obj.ProgID == id & obj.BookletSize == bookletsize).AsQueryable();

            if (progConfig == null || !progConfig.Any())
            {
                return NotFound();
            }

            return Ok(progConfig);
        }




        // PUT: api/ProgConfigs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProgConfig(int id, ProgConfig progConfig)
        {
            if (id != progConfig.ProgConfigID)
            {
                return BadRequest();
            }

            _context.Entry(progConfig).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Program Configuration Updated for {progConfig.ProgID}", "ProgConfig", userId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgConfigExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error occurred while updating Program-Configuration", "DbUpdateConcurrencyException", "ProgConfigsController");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProgConfigs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProgConfig>> PostProgConfig([FromBody] ProgConfigInput progConfig)
        {
            if (_context.ProgConfigs == null)
            {
                return Problem("Entity set 'KeyGeneratorDBContext.PaperConfigs'  is null.");
            }

            ProgConfig progConfig1 = new ProgConfig()
            {
                ProgID = progConfig.ProgID,
                Sets = progConfig.Sets,
                SetOrder = progConfig.SetOrder,
                NumberofQuestions = progConfig.NumberofQuestions,
                BookletSize = progConfig.BookletSize,
                NumberofJumblingSteps = progConfig.NumberofJumblingSteps,
                SetofStepsID = progConfig.SetofStepsID,
            };
            _context.ProgConfigs.Add(progConfig1);

            await _context.SaveChangesAsync();

            int generatedid = progConfig1.ProgConfigID;
            for (int i = 0; i < progConfig.setofSteps.Length; i++)
            {
                SetofStep set = new SetofStep()
                {
                    ProgConfigID = generatedid,
                    steps = progConfig.setofSteps[i],
                    StepID = i + 1,
                };
                _context.SetofSteps.Add(set);
            }
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"Program Configuration: for Program-ID {progConfig1.ProgID} Added ", "ProgConfig", userId);
                _logger.LogEvent($"Set Of Steps : for Program-Configuration-ID {progConfig1.ProgConfigID} Added ", "SetofSteps", userId);
            }

            return Ok(new {progConfig1.ProgConfigID});
        }

        // DELETE: api/ProgConfigs/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgConfig(int id)
        {
            var progConfig = await _context.ProgConfigs.FindAsync(id);
            if (progConfig == null)
            {
                return NotFound();
            }
            var setofsteps = _context.SetofSteps.Where(u => u.ProgConfigID == id).ToList();
            foreach (var step in setofsteps)
            {
                _context.SetofSteps.Remove(step);
            }

            _context.ProgConfigs.Remove(progConfig);

            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"Program Configuration: for Program-ID {progConfig.ProgID} Deleted ", "ProgConfig", userId);
                _logger.LogEvent($"Set Of Steps : for Program-Configuration-ID {progConfig.ProgConfigID} Deleted ", "SetofSteps", userId);
            }

            return NoContent();
        }

        private bool ProgConfigExists(int id)
        {
            return _context.ProgConfigs.Any(e => e.ProgConfigID == id);
        }
    }
}
