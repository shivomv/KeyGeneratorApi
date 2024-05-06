﻿using System;
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
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaperDetails>>> GetPapers()
        {
            var papers = await _context.Papers
                .GroupJoin(
                    _context.Programmes,
                    p => p.ProgrammeID,
                    prg => prg.ProgrammeID,
                    (p, prgs) => new { Paper = p, Programmes = prgs }
                )
                .SelectMany(
                    x => x.Programmes.DefaultIfEmpty(),
                    (x, prg) => new { Paper = x.Paper, Programme = prg }
                )
                .GroupJoin(
                    _context.Courses,
                    pc => pc.Paper.CourseID,
                    c => c.CourseID,
                    (pc, cs) => new { pc.Paper, pc.Programme, Courses = cs }
                )
                .SelectMany(
                    x => x.Courses.DefaultIfEmpty(),
                    (x, c) => new { x.Paper, x.Programme, Course = c }
                )
                .GroupJoin(
                    _context.Subjects,
                    pcs => pcs.Paper.SubjectID,
                    s => s.SubjectID,
                    (pcs, ss) => new { pcs.Paper, pcs.Programme, pcs.Course, Subjects = ss }
                )
                .SelectMany(
                    x => x.Subjects.DefaultIfEmpty(),
                    (x, s) => new { x.Paper, x.Programme, x.Course, Subject = s }
                )
                .GroupJoin(
                    _context.Users,
                    puc => puc.Paper.CreatedByID,
                    u => u.UserID,
                    (puc, us) => new { puc.Paper, puc.Programme, puc.Course, puc.Subject, Users = us }
                )
                .SelectMany(
                    x => x.Users.DefaultIfEmpty(),
                    (x, u) => new { x.Paper, x.Programme, x.Course, x.Subject, CreatedBy = u }
                )
                .Select(p => new PaperDetails
                {
                    PaperID = p.Paper.PaperID,
                    ProgrammeID = p.Paper.ProgrammeID,
                    PaperName = p.Paper.PaperName,
                    CatchNumber = p.Paper.CatchNumber,
                    PaperCode = p.Paper.PaperCode,
                    CourseID = p.Paper.CourseID ?? 0,
                    ExamType = p.Paper.ExamType,
                    SubjectID = p.Paper.SubjectID ?? 0,
                    PaperNumber = p.Paper.PaperNumber,
                    ExamDate = p.Paper.ExamDate,
                    NumberofQuestion = p.Paper.NumberofQuestion ?? 0,
                    BookletSize = p.Paper.BookletSize ?? 0,
                    CreatedAt = p.Paper.CreatedAt,
                    CreatedByID = p.Paper.CreatedByID,
                    MasterUploaded = p.Paper.MasterUploaded,
                    KeyGenerated = p.Paper.KeyGenerated,
                    ProgrammeName = p.Programme != null ? p.Programme.ProgrammeName : "",
                    CourseName = p.Course != null ? p.Course.CourseName : "",
                    SubjectName = p.Subject != null ? p.Subject.SubjectName : "",
                    CreatedBy = p.CreatedBy != null ? p.CreatedBy.FirstName : ""
                })
                .ToListAsync();

            return Ok(papers);
        }

        // GET: api/Papers/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPaper(int id)
        {
            var paper = await _context.Papers.FindAsync(id);

            if (paper == null)
            {
                return NotFound();
            }

            var paperDetails = new
            {
                paper.PaperID,
                paper.ProgrammeID,
                paper.PaperName,
                paper.CatchNumber,
                paper.PaperCode,
                paper.CourseID,
                paper.ExamType,
                paper.SubjectID,
                paper.PaperNumber,
                paper.ExamDate,
                paper.NumberofQuestion,
                paper.BookletSize,
                paper.CreatedAt,
                paper.CreatedByID,
                paper.MasterUploaded,
                paper.KeyGenerated,
                ProgrammeName = _context.Programmes.FirstOrDefault(p => p.ProgrammeID == paper.ProgrammeID)?.ProgrammeName,
                CourseName = _context.Courses.FirstOrDefault(c => c.CourseID == paper.CourseID)?.CourseName,
                SubjectName = _context.Subjects.FirstOrDefault(s => s.SubjectID == paper.SubjectID)?.SubjectName,
                CreatedBy = _context.Users.FirstOrDefault(u => u.UserID == paper.CreatedByID)?.FirstName
            };

            return Ok(paperDetails);
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

        [HttpGet("Program/{id}")]
        public async Task<ActionResult<IEnumerable<PaperDetails>>> GetPaperByProgramID(int id)
        {
            var papers = await _context.Papers
                .Where(u => u.ProgrammeID == id)
                .GroupJoin(
                    _context.Programmes,
                    p => p.ProgrammeID,
                    prg => prg.ProgrammeID,
                    (p, prgs) => new { Paper = p, Programmes = prgs }
                )
                .SelectMany(
                    x => x.Programmes.DefaultIfEmpty(),
                    (x, prg) => new { x.Paper, Programme = prg }
                )
                .GroupJoin(
                    _context.Courses,
                    pc => pc.Paper.CourseID,
                    c => c.CourseID,
                    (pc, cs) => new { pc.Paper, pc.Programme, Courses = cs }
                )
                .SelectMany(
                    x => x.Courses.DefaultIfEmpty(),
                    (x, c) => new { x.Paper, x.Programme, Course = c }
                )
                .GroupJoin(
                    _context.Subjects,
                    pcs => pcs.Paper.SubjectID,
                    s => s.SubjectID,
                    (pcs, ss) => new { pcs.Paper, pcs.Programme, pcs.Course, Subjects = ss }
                )
                .SelectMany(
                    x => x.Subjects.DefaultIfEmpty(),
                    (x, s) => new { x.Paper, x.Programme, x.Course, Subject = s }
                )
                .GroupJoin(
                    _context.Users,
                    puc => puc.Paper.CreatedByID,
                    u => u.UserID,
                    (puc, us) => new { puc.Paper, puc.Programme, puc.Course, puc.Subject, Users = us }
                )
                .SelectMany(
                    x => x.Users.DefaultIfEmpty(),
                    (x, u) => new { x.Paper, x.Programme, x.Course, x.Subject, CreatedBy = u }
                )
                .Select(p => new PaperDetails
                {
                    PaperID = p.Paper.PaperID,
                    ProgrammeID = p.Paper.ProgrammeID,
                    PaperName = p.Paper.PaperName,
                    CatchNumber = p.Paper.CatchNumber,
                    PaperCode = p.Paper.PaperCode,
                    CourseID = p.Paper.CourseID ?? 0,
                    ExamType = p.Paper.ExamType,
                    SubjectID = p.Paper.SubjectID ?? 0,
                    PaperNumber = p.Paper.PaperNumber,
                    ExamDate = p.Paper.ExamDate,
                    NumberofQuestion = p.Paper.NumberofQuestion ?? 0,
                    BookletSize = p.Paper.BookletSize ?? 0,
                    CreatedAt = p.Paper.CreatedAt,
                    CreatedByID = p.Paper.CreatedByID,
                    MasterUploaded = p.Paper.MasterUploaded,
                    KeyGenerated = p.Paper.KeyGenerated,
                    ProgrammeName = p.Programme != null ? p.Programme.ProgrammeName : "",
                    CourseName = p.Course != null ? p.Course.CourseName : "",
                    SubjectName = p.Subject != null ? p.Subject.SubjectName : "",
                    CreatedBy = p.CreatedBy != null ? p.CreatedBy.FirstName : ""
                })
                .ToListAsync();

            if (papers == null || papers.Count == 0)
            {
                return NotFound();
            }

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

            var pendingCount = allPapersCount - masterUploadedCount;

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

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Paper>>> SearchPapers([FromQuery] string searchData)
        {
            var papers = await _context.Papers
                .GroupJoin(
                    _context.Programmes,
                    p => p.ProgrammeID,
                    prg => prg.ProgrammeID,
                    (p, prgs) => new { Paper = p, Programmes = prgs }
                )
                .SelectMany(
                    x => x.Programmes.DefaultIfEmpty(),
                    (x, prg) => new { Paper = x.Paper, Programme = prg }
                )
                .GroupJoin(
                    _context.Courses,
                    pc => pc.Paper.CourseID,
                    c => c.CourseID,
                    (pc, cs) => new { pc.Paper, pc.Programme, Courses = cs }
                )
                .SelectMany(
                    x => x.Courses.DefaultIfEmpty(),
                    (x, c) => new { x.Paper, x.Programme, Course = c }
                )
                .GroupJoin(
                    _context.Subjects,
                    pcs => pcs.Paper.SubjectID,
                    s => s.SubjectID,
                    (pcs, ss) => new { pcs.Paper, pcs.Programme, pcs.Course, Subjects = ss }
                )
                .SelectMany(
                    x => x.Subjects.DefaultIfEmpty(),
                    (x, s) => new { x.Paper, x.Programme, x.Course, Subject = s }
                )
                .GroupJoin(
                    _context.Users,
                    puc => puc.Paper.CreatedByID,
                    u => u.UserID,
                    (puc, us) => new { puc.Paper, puc.Programme, puc.Course, puc.Subject, Users = us }
                )
                .SelectMany(
                    x => x.Users.DefaultIfEmpty(),
                    (x, u) => new { x.Paper, x.Programme, x.Course, x.Subject, CreatedBy = u }
                )
                .Where(p =>
                    p.Paper.PaperName.Contains(searchData) ||
                    p.Paper.CatchNumber.Contains(searchData) ||
                    p.Paper.PaperCode.Contains(searchData) ||
                    p.Paper.ExamType.Contains(searchData) ||
                    // Add other fields here
                    p.Programme.ProgrammeName.Contains(searchData) ||
                    p.Course.CourseName.Contains(searchData) ||
                    p.Subject.SubjectName.Contains(searchData) ||
                    p.CreatedBy.FirstName.Contains(searchData)
                )
                .Select(p => new PaperDetails
                {
                    PaperID = p.Paper.PaperID,
                    ProgrammeID = p.Paper.ProgrammeID,
                    PaperName = p.Paper.PaperName,
                    CatchNumber = p.Paper.CatchNumber,
                    PaperCode = p.Paper.PaperCode,
                    CourseID = p.Paper.CourseID ?? 0,
                    ExamType = p.Paper.ExamType,
                    SubjectID = p.Paper.SubjectID ?? 0,
                    PaperNumber = p.Paper.PaperNumber,
                    ExamDate = p.Paper.ExamDate,
                    NumberofQuestion = p.Paper.NumberofQuestion ?? 0,
                    BookletSize = p.Paper.BookletSize ?? 0,
                    CreatedAt = p.Paper.CreatedAt,
                    CreatedByID = p.Paper.CreatedByID,
                    MasterUploaded = p.Paper.MasterUploaded,
                    KeyGenerated = p.Paper.KeyGenerated,
                    ProgrammeName = p.Programme != null ? p.Programme.ProgrammeName : "",
                    CourseName = p.Course != null ? p.Course.CourseName : "",
                    SubjectName = p.Subject != null ? p.Subject.SubjectName : "",
                    CreatedBy = p.CreatedBy != null ? p.CreatedBy.FirstName : ""
                })
                .ToListAsync();

            if (papers == null || papers.Count == 0)
            {
                return NotFound();
            }

            return Ok(papers);
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
