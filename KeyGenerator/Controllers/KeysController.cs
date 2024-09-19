using KeyGenerator.Data;
using KeyGenerator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeyGenerator.Models.NonDBModels;
using Microsoft.AspNetCore.Authorization;
using KeyGenerator.Models;
using System.Security.Claims;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;
        public KeysController(KeyGeneratorDBContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize]
        [HttpPost("UpdateAnswers")]
        public async Task<IActionResult> UpdateAnswers([FromBody] List<AnswerUpdate> answerUpdates)
        {


            foreach (var answerUpdate in answerUpdates) //  {"paperID": 2,"questionNumber": 1,"answer": "V","previousAnswer": "B"}
            {
                var answers = await _context.AnswersKeys.Where(u => u.PaperID == answerUpdate.PaperID && u.QuestionNumber == answerUpdate.QuestionNumber).ToListAsync();
                if (answers == null)
                {
                    return NotFound();
                }
                foreach (var answer in answers)
                {
                    answer.Answer = answerUpdate.Answer;
                }

                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Answer Updated for Question: {answerUpdate.QuestionNumber} From {answerUpdate.PreviousAnswer} to {answerUpdate.Answer} in Paper Id:  {answerUpdate.PaperID}", "Keys", userId);
                }

            }
            _context.SaveChanges();


            return Ok("Updated");

        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteKeys(int PaperID, string catchNumber)
        {
            var answers = _context.AnswersKeys.Where(u => u.PaperID == PaperID).ToList();
            if (answers == null || !answers.Any())
            {
                return NotFound();
            }

            _context.AnswersKeys.RemoveRange(answers);

            // Update Paper entity to set MasterUploaded and KeyGenerated to false
            var paper = _context.Papers.FirstOrDefault(p => p.PaperID == PaperID);
            if (paper != null)
            {
                paper.MasterUploaded = false;
                paper.KeyGenerated = false;
            }

            await _context.SaveChangesAsync();

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogEvent($"Answer Key for Catch Number : {catchNumber} Deleted", "Keys", userId);
            }
            return Ok("Deleted");
        }

    }
}
