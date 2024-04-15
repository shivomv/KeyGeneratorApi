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
using KeyGenerator.Models.NonDBModels;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PapersController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        public PapersController(KeyGeneratorDBContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Papers
        // GET: api/Papers
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paper>>> GetPapers()
        {
            var papers = await _context.Papers.ToListAsync();

            var paperDetails = papers.Select(p => new
            {
                p.PaperID,
                p.ProgrammeID,
                p.PaperName,
                p.CatchNumber,
                p.PaperCode,
                p.CourseID,
                p.ExamType,
                p.SubjectID,
                p.PaperNumber,
                p.ExamDate,
                p.NumberofQuestion,
                p.BookletSize,
                p.CreatedAt,
                p.CreatedByID,
                p.MasterUploaded,
                p.KeyGenerated,
                ProgrammeName = _context.Programmes.FirstOrDefault(p => p.ProgrammeID == p.ProgrammeID)?.ProgrammeName,
                CourseName = _context.Courses.FirstOrDefault(c => c.CourseID == p.CourseID)?.CourseName,
                SubjectName = _context.Subjects.FirstOrDefault(s => s.SubjectID == p.SubjectID)?.SubjectName,
                CreatedBy = _context.Users.FirstOrDefault(u => u.UserID == p.CreatedByID)?.FirstName
            });

            return Ok(paperDetails);
        }


        // GET: api/Papers/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Paper>> GetPaper(int id)
        {
            var paper = await _context.Papers.FindAsync(id);

            if (paper == null)
            {
                return NotFound();
            }

            return paper;
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Paper>>> PostPapers(IEnumerable<Paper> papers)
        {
            foreach (var paper in papers)
            {
                _context.Papers.Add(paper);
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Paper: {paper.PaperName} Added ", "Paper", userId);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(papers);
        }



        [HttpGet("Programme/{id}")]
        public async Task<ActionResult<IEnumerable<Paper>>> GetPaperByProgrammeID(int id)
        {
            var papers = await _context.Papers.Where(u => u.ProgrammeID == id).ToListAsync();

            if (papers == null || papers.Count == 0)
            {
                return NotFound();
            }

            return papers;
        }

        // PUT: api/Papers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaper(int id, Paper paper)
        {
            if (id != paper.PaperID)
            {
                return BadRequest();
            }

            _context.Entry(paper).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Paper: {paper.PaperName} Updated", "Paper", userId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaperExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error occurred while updating Paper", "DbUpdateConcurrencyException", "PapersController");
                    throw;
                }
            }

            return NoContent();
        }

        [HttpGet("StatusCount")]
        public async Task<ActionResult<StatusCount>> StatusCount()
        {
            var papersStatus = await _context.Papers
                .GroupBy(p => p.KeyGenerated)
                .Select(g => new { KeyGenerated = g.Key, Count = g.Count() })
                .ToListAsync();

            var userCount = await _context.Users.CountAsync();
            var groupCount = await _context.Groups.CountAsync();
            var allPapersCount = await _context.Papers.CountAsync();

            var keyGeneratedCount = papersStatus.FirstOrDefault(s => s.KeyGenerated)?.Count ?? 0;

            var masterUploadedCount = await _context.Papers
                .CountAsync(p => p.MasterUploaded);

            var pendingCount = allPapersCount - keyGeneratedCount - masterUploadedCount;

            var statusCount = new StatusCount
            {
                UserCount = userCount,
                GroupCount = groupCount,
                AllPapersCount = allPapersCount,
                KeyGenerated = keyGeneratedCount,
                MasterUploaded = masterUploadedCount,
                Pendingkeys = pendingCount
            };

            return statusCount;
        }

        //// POST: api/Papers
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize]
        //[HttpPost]
        //public async Task<ActionResult<Paper>> PostPaper(Paper paper)
        //{
        //    _context.Papers.Add(paper);
        //    await _context.SaveChangesAsync();
        //    var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        //    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        //    {
        //        // Now you have the user ID
        //        _logger.LogEvent($"Paper: {paper.PaperName} Added ", "Paper", userId);
        //    }

        //    return CreatedAtAction("GetPaper", new { id = paper.PaperID }, paper);
        //}

        // DELETE: api/Papers/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaper(int id)
        {
            var paper = await _context.Papers.FindAsync(id);
            if (paper == null)
            {
                return NotFound();
            }

            _context.Papers.Remove(paper);
            await _context.SaveChangesAsync();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"Paper: {paper.PaperName} Deleted ", "Paper", userId);
            }

            return NoContent();
        }

        private bool PaperExists(int id)
        {
            return _context.Papers.Any(e => e.PaperID == id);
        }
    }
}
