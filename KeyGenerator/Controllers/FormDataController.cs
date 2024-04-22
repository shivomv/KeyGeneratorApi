using KeyGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using KeyGenerator.Data;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using KeyGenerator.Services;
using System.Web.Helpers;
using CsvHelper;
using System.Globalization;
using KeyGenerator.Models.NonDBModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormDataController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        List<string> FilePaths = new List<string>();

        public FormDataController(KeyGeneratorDBContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        //post master file
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadAndSaveCSV(int ProgID, string CatchNumber, int PaperID)
        {
            try
            {
                var file = Request.Form.Files[0];
                var prog = await _context.Programmes.FindAsync(ProgID);
                if(prog==null)
                {
                    return NotFound();
                }
                var paperinfo = await _context.Papers.FindAsync(PaperID);
                

                if (file != null && file.Length > 0)
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        int i = 1;
                        csv.Read();
                        csv.ReadHeader();
                        while (csv.Read())
                        {
                            int pageNumber = csv.GetField<int>("Page No.");
                            int questionNumber = csv.GetField<int>("Q#");
                            string answer = csv.GetField<string>("Key");

                            Answers answerKeyData = new Answers()
                            {
                                SerialNumber = i,
                                PageNumber = pageNumber,
                                QuestionNumber = questionNumber,
                                Answer = answer,
                                ProgID = ProgID,
                                CatchNumber = CatchNumber, // Assuming CatchNumber is defined somewhere
                                PaperID = paperinfo.PaperID,
                                SetID = 1
                            };
                            _context.AnswersKeys.Add(answerKeyData);

                            i++;
                        }
                        await _context.SaveChangesAsync();
                    }
                    paperinfo.MasterUploaded = true;
                    await _context.SaveChangesAsync();
                    var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        // Now you have the user ID
                        _logger.LogEvent($"Master Key Uploaded for {CatchNumber}", "Key", userId);
                    }

                    return Ok(new { prog,CatchNumber, paperinfo.PaperID});
                }
                else
                {
                    return BadRequest("No file or empty file received.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error", $"{ex.Message}", "FormDataControllerUSCSV");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize]
        [HttpPost("GenerateKey")]
        public IActionResult GenerateKey(KeyEssential KeyInput)
        {


            static List<List<string>> FilterNestedList(List<List<string>> data, string searchElement)
            {
                return data.Where(subList => subList[0] == searchElement).ToList();
            }

            static List<List<string>> PageNumberOrder(List<List<string>> example)
            {
                Dictionary<string, string> pageMapping = new Dictionary<string, string>();
                int currentPage = 3;

                foreach (var row in example)
                {
                    string pageNo = row[0];
                    if (!pageMapping.ContainsKey(pageNo))
                    {
                        pageMapping[pageNo] = currentPage.ToString();
                        currentPage++;
                    }
                }

                foreach (var row in example)
                {
                    row[0] = pageMapping[row[0]];
                }

                return example;
            }

            static List<List<string>> Jumble(List<KeyData> keys, int iterations, List<string[]> setOfSteps)
            {
                List<List<string>> dataList = new List<List<string>>();
                List<List<string>> tempDataList = new List<List<string>>();
                List<List<string>> jumbledDataList = new List<List<string>>();

                foreach (var key in keys)
                {
                    dataList.Add(new List<string>
            {
                key.PageNumber.ToString(),
                key.QuestionNumber.ToString(),
                key.Answer
            });
                }

                for (int i = 0; i < iterations; i++)
                {
                    var steps = setOfSteps[i];
                    foreach (var j in steps)
                    {
                        tempDataList = FilterNestedList(dataList, j);
                        foreach (var line in tempDataList)
                        {
                            jumbledDataList.Add(line);
                        }
                        dataList = dataList.Where(line => line[0] != j).ToList();
                    }

                    jumbledDataList.AddRange(dataList);
                    dataList = new List<List<string>>(jumbledDataList);
                    jumbledDataList.Clear();
                }

                dataList = PageNumberOrder(dataList);

                return dataList;
            }

            try
            {
                int copies = KeyInput.Copies;
                int setid = KeyInput.SetID; //1
                for (int i = 1; i <= copies - 1; i++)
                {
                    var keys = _context.AnswersKeys
                        .Where(k => k.ProgID == KeyInput.ProgID && k.PaperID == KeyInput.PaperID && k.CatchNumber == KeyInput.CatchNumber && k.SetID == setid)
                        .Select(k => new KeyData
                        {
                            PageNumber = k.PageNumber,
                            QuestionNumber = k.QuestionNumber,
                            Answer = k.Answer
                        })
                        .ToList();

                    int iterations = KeyInput.Iterations;
                    List<string[]> setOfSteps = KeyInput.SetofSteps;
                    List<List<string>> dataList = Jumble(keys, iterations, setOfSteps);
                    int j = 1;
                    foreach (var item in dataList)
                    {
                        var pageNumber = int.Parse(item[0]);
                        var questionNumber = int.Parse(item[1]);
                        var answer = item[2];

                        Answers answerKeyData = new Answers()
                        {
                            SerialNumber = j,
                            PageNumber = pageNumber,
                            QuestionNumber = questionNumber,
                            Answer = answer,
                            ProgID = KeyInput.ProgID,
                            CatchNumber = KeyInput.CatchNumber,
                            PaperID = KeyInput.PaperID,
                            SetID = (setid + 1)
                        };

                        _context.AnswersKeys.Add(answerKeyData);
                        j++;
                    }

                    _context.SaveChanges();
                    setid++;
                }
                var prog = _context.Programmes.FirstOrDefault(u => u.ProgrammeID == KeyInput.ProgID); if (prog == null)
                {
                    return NotFound();
                }

                var Uni = _context.Groups.FirstOrDefault(u => u.GroupID == prog.GroupID);
                /*var Sub = _context.Subjects.FirstOrDefault(u => u.SubjectID == KeyInput.SubjectID);*/
                var paper = _context.Papers.FirstOrDefault(u => u.CatchNumber == KeyInput.CatchNumber);
                paper.KeyGenerated = true;
                _context.SaveChanges();
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Key-Generated for {KeyInput.CatchNumber}", "Key", userId);
                }

                return Ok(new { prog.ProgrammeID, KeyInput.CatchNumber, KeyInput.PaperID });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Generating Key ", $"{ex.Message}", "FormDataControllerGENKEY");
                return BadRequest(ex);
            }

        }

        // get generated files
        [Authorize]
        [HttpGet]
        public IActionResult GetByName(int progID, string CatchNumber, int PaperID)
        {

            try
            {
                var prog = _context.Programmes.Find(progID);
                /*var subject = _context.Subjects.FirstOrDefault(u => u.SubjectName == Subject);*/
                var group = _context.Groups.FirstOrDefault(i => i.GroupID == prog.GroupID);
                var keys = _context.AnswersKeys
    .Where(k => k.ProgID== progID && k.PaperID == PaperID && k.CatchNumber == CatchNumber)
    .Select(k => new
    {
        pageNumber = k.PageNumber,
        questionNumber = k.QuestionNumber,
        answer = k.Answer,
        setID = k.SetID
    })
    .ToList();


                if (keys == null)
                {
                    return NotFound();
                }
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Now you have the user ID
                    _logger.LogEvent($"Master Key Uploaded", "Key",userId);
                }
                return Ok(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Accessing Key ", $"{ex.Message}", "FormDataControllerGetKey");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
