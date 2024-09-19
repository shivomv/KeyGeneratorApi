using Amazon;
using Amazon.SimpleEmail.Model;
using KeyGenerator.Data;
using KeyGenerator.Models;
using KeyGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PDFfileController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _dbContext;
        private readonly ILoggerService _logger;
        public PDFfileController(KeyGeneratorDBContext dBContext, ILoggerService logger)
        {
            _dbContext = dBContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> PostPDFFile([FromForm] InputPdf inputPdf, int ProgramId)
        {
            if (inputPdf.File == null)
            {
                return BadRequest("File is null");
            }

            // Ensure the wwwroot/CatchPdfs directory exists
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CatchPdfs");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Generate a unique file name to avoid overwriting existing files
            var uniqueFileName = $"{ProgramId}_{inputPdf.File.FileName}";
            var absoluteFilePath = Path.Combine(uploadsFolderPath, uniqueFileName); // Full path for saving the file
            var relativeFilePath = Path.Combine("CatchPdfs", uniqueFileName); // Relative path to store in the database

            // Extract CatchNumber and SeriesName from the filename
            string catchNumber = inputPdf.File.FileName.Split('_')[0];
            var paper = _dbContext.Papers.FirstOrDefault(u => u.CatchNumber == catchNumber && u.ProgrammeID == ProgramId);
            if (paper == null)
            {
                return BadRequest("No Paper Found for this Catch Number");
            }
            string seriesName = inputPdf.File.FileName.Split('_')[1].Split('.')[0];

            var existingPdf = await _dbContext.PDFfiles
        .FirstOrDefaultAsync(p => p.FileName == uniqueFileName && p.ProgramId == ProgramId);
            if (existingPdf != null)
            {
                return BadRequest("PDF already exists");
            }

            // Save the file to the server
            using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
            {
                await inputPdf.File.CopyToAsync(stream);
            }

            // Create a new PDFfile object and save it to the database
            PDFfile pDFfile = new PDFfile()
            {
                FileName = uniqueFileName,
                FilePath = relativeFilePath,  // Store only the relative path in the database
                CatchNumber = catchNumber,
                SeriesName = seriesName,
                ProgramId = ProgramId
            };
            try
            {
                _dbContext.PDFfiles.Add(pDFfile);
                await _dbContext.SaveChangesAsync();
                

            }
            catch (DbUpdateException)
            {
                return BadRequest("FileName Already Exists in this Program");
            }

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent($"PDF File Uploaded: {uniqueFileName}", "CatchUpload", userId);
            }

            return Ok(pDFfile);
        }

        /*[AllowAnonymous]
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            int userID = 0;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                userID = userId;
            }

            // Retrieve all PDF files that match the provided CatchNumber and ProgramId
            var pdfFiles = await _dbContext.PDFfiles
                .Where(p => p.CatchNumber == request.CatchNumber && p.ProgramId == request.ProgramId)
                .ToListAsync();

            if (pdfFiles.Count == 0)
            {
                return NotFound("No PDF files found for the given CatchNumber and ProgramId.");
            }

            // Update status for all matching PDF files
            foreach (var pdfFile in pdfFiles)
            {
                pdfFile.Status = request.Status;
                pdfFile.VerifiedBy = userID;

                // Convert UTC to Indian Standard Time (IST)
                var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                pdfFile.VerifiedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);

                // Optionally, add additional logic if needed
            }

            try
            {
                _dbContext.PDFfiles.UpdateRange(pdfFiles); // Use UpdateRange to update multiple records
                await _dbContext.SaveChangesAsync();

                // Aggregate logs
                var uniqueLogs = pdfFiles
                    .GroupBy(p => new { p.CatchNumber, p.SeriesName })
                    .Select(g => new
                    {
                        CatchNumber = g.Key.CatchNumber,
                        SeriesName = g.Key.SeriesName
                    });

                foreach (var log in uniqueLogs)
                {
                    _logger.LogEvent("Catch: {CatchNumber} Series: {SeriesName} Verified", "Verification", userID);
                }

                return Ok("Status updated successfully for all matching PDF files.");
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError("Error occurred while updating Paper", "DbUpdateConcurrencyException", "PapersController");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating PDF files: {ex.Message}");
            }
        }*/


        [AllowAnonymous]
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            int userID = 0;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                userID = userId;
            }

            // Retrieve all PDF files that match the provided CatchNumber and ProgramId
            var pdfFiles = await _dbContext.PDFfiles
                .Where(p => p.CatchNumber == request.CatchNumber && p.ProgramId == request.ProgramId)
                .ToListAsync();

            if (pdfFiles.Count == 0)
            {
                return NotFound("No PDF files found for the given CatchNumber and ProgramId.");
            }

            // Use HashSet to avoid duplicate entries
            var uniqueLogs = new HashSet<(string CatchNumber, string SeriesName)>();

            // Update status for all matching PDF files
            foreach (var pdfFile in pdfFiles)
            {
                pdfFile.Status = request.Status;
                pdfFile.VerifiedBy = userID;

                // Convert UTC to Indian Standard Time (IST)
                var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                pdfFile.VerifiedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);

                // Add unique log entry to HashSet
                uniqueLogs.Add((pdfFile.CatchNumber, pdfFile.SeriesName));
            }

            try
            {
                _dbContext.PDFfiles.UpdateRange(pdfFiles); // Use UpdateRange to update multiple records
                await _dbContext.SaveChangesAsync();

                // Log the unique logs
                foreach (var log in uniqueLogs)
                {
                    _logger.LogEvent($"Catch: {log.CatchNumber} Series: {log.SeriesName} Verified", "Verification", userID);
                }

                return Ok("Status updated successfully for all matching PDF files.");
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError("Error occurred while updating Paper", "DbUpdateConcurrencyException", "PapersController");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating PDF files: {ex.Message}");
            }
        }





        [AllowAnonymous]
        [HttpGet("GetPdfsWithPagination")]
        public async Task<ActionResult> GetPdfsWithPagination(
      int programId,
      int pageNumber = 1,
      int pageSize = 10,
      string sortField = "catchNumber",
      string sortOrder = "asc",
      string searchQuery = ""  // Search parameter for CatchNumber
  )
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid page number or page size.");
            }

            // Step 1: Get the catch numbers, sorted and limited
            var catchNumbersQuery = _dbContext.PDFfiles
                .Where(u => u.ProgramId == programId &&
                            (string.IsNullOrEmpty(searchQuery) || u.CatchNumber.Contains(searchQuery))) // Add search filter
                .GroupBy(u => u.CatchNumber)
                .Select(g => new
                {
                    CatchNumber = g.Key
                });

            // Apply sorting based on the sortField and sortOrder
            var sortedCatchNumbers = sortField switch
            {
                "catchNumber" => sortOrder == "desc" ? catchNumbersQuery.OrderByDescending(e => e.CatchNumber) : catchNumbersQuery.OrderBy(e => e.CatchNumber),
                _ => catchNumbersQuery // Default sorting if no match
            };

            var totalCatchNumbers = await sortedCatchNumbers.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCatchNumbers / (double)pageSize);

            // Get the paginated results
            var topCatchNumbers = await sortedCatchNumbers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var catchNumberList = topCatchNumbers.Select(cn => cn.CatchNumber).ToList();

            // Step 2: Fetch PDFs for the selected catch numbers with user details
            var pdfs = await _dbContext.PDFfiles
                .Where(u => u.ProgramId == programId && catchNumberList.Contains(u.CatchNumber))
                .Join(_dbContext.Users, // Assuming the User table is joined here
                      pdf => pdf.VerifiedBy,
                      user => user.UserID,
                      (pdf, user) => new
                      {
                          pdf.Id,
                          pdf.FileName,
                          pdf.FilePath,
                          pdf.CatchNumber,
                          pdf.SeriesName,
                          pdf.ProgramId,
                          pdf.Status,
                          VerifiedByName = $"{user.FirstName} {user.MiddleName} {user.LastName}",
                          pdf.VerifiedAt
                      })
                .ToListAsync();

            // Step 3: Group PDFs by catch number (no limit on number of PDFs per catch number)
            var groupedPdfs = pdfs
                .GroupBy(p => p.CatchNumber)
                .Select(g => new
                {
                    CatchNumber = g.Key,
                    Pdfs = g.ToList() // No limit on number of PDFs per catch number
                })
                .ToList();

            // Step 4: Ensure exactly the requested number of catch numbers in the response
            var result = topCatchNumbers
                .Select(cn => new
                {
                    CatchNumber = cn.CatchNumber,
                    Pdfs = groupedPdfs.FirstOrDefault(g => g.CatchNumber == cn.CatchNumber)
                })
                .ToList();

            // Create the response with pagination metadata
            var response = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCatchNumbers, // Total number of catch numbers
                TotalPages = totalPages,
                Data = result
            };

            return Ok(response);
        }




        [HttpGet("GetPdfs")]
        public async Task<ActionResult> GetPdfs(string CatchNumber, int ProgramId)
        {
            List<PDFfile> pdffiles = await _dbContext.PDFfiles.Where(u => u.ProgramId == ProgramId && u.CatchNumber == CatchNumber).ToListAsync();
            return Ok(pdffiles);
        }


        [HttpPost("VerifyPageNumber")]
        public async Task<ActionResult> VerifyPageNumber(FileVerification fileVerification)
        {
            int userID = 0;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                userID = userId;
            }
            var existingentry = _dbContext.fileVerifications.FirstOrDefault(u => u.CatchNumber == fileVerification.CatchNumber &&
            u.PageNumber == fileVerification.PageNumber && u.ProgramId == fileVerification.ProgramId && u.SeriesName == fileVerification.SeriesName);

            if (existingentry != null)
            {
                existingentry.VerifiedBy = userID;
                existingentry.IsCorrect = fileVerification.IsCorrect;

                try
                {
                    _dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return Ok(existingentry);
            }

            if (fileVerification == null)
            {
                return BadRequest();
            }
            try
            {
                _dbContext.fileVerifications.Add(fileVerification);
                _dbContext.SaveChanges();
                //_logger.LogEvent($"{}", "VerifyPageNumber", userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(fileVerification);
        }

        /* [AllowAnonymous]
         [HttpGet("pageVerificationStatus")]
         public async Task<ActionResult<IEnumerable<FileVerification>>> getAllentries(string CatchNumber, int ProgramId)
         {
             var listofentries = await _dbContext.fileVerifications.Where(u => u.CatchNumber == CatchNumber && u.ProgramId == ProgramId).Select().ToListAsync();

             return Ok(listofentries);
         }*/
        [AllowAnonymous]
        [HttpGet("pageVerificationStatus")]
        public async Task<ActionResult<IEnumerable<object>>> getAllentries(string CatchNumber, int ProgramId)
        {
            var listofentries = await _dbContext.fileVerifications
                .Where(u => u.CatchNumber == CatchNumber && u.ProgramId == ProgramId)
                .Select(u => new
                {
                    u.PageNumber,
                    u.IsCorrect,
                    u.SeriesName
                })
                .ToListAsync();

            return Ok(listofentries);
        }


        [AllowAnonymous]
        [HttpGet("ExportStatusToExcel")]
        public async Task<IActionResult> ExportStatusToExcel(int programId)
        {
            // Fetch and sort data from the database using a join to include user information
            var fileVerifications = await (from pdf in _dbContext.PDFfiles
                                           join user in _dbContext.Users
                                           on pdf.VerifiedBy equals user.UserID into userJoin
                                           from user in userJoin.DefaultIfEmpty()
                                           where pdf.ProgramId == programId
                                           orderby pdf.CatchNumber, pdf.SeriesName, pdf.Status
                                           select new
                                           {
                                               pdf.CatchNumber,
                                               pdf.SeriesName,
                                               pdf.Status,
                                               VerifiedByName = user != null ? $"{user.FirstName} {user.LastName}" : "N/A",
                                               pdf.VerifiedAt
                                           })
                                            .ToListAsync();

            if (fileVerifications == null || !fileVerifications.Any())
            {
                return NotFound("No data available for the given parameters.");
            }

            // Create a new Excel package
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Status Report");

            // Add headers
            worksheet.Cells[1, 1].Value = "Catch Number";
            worksheet.Cells[1, 2].Value = "Series Name";
            worksheet.Cells[1, 3].Value = "Status";
            worksheet.Cells[1, 4].Value = "Verified By";
            worksheet.Cells[1, 5].Value = "Verified At";

            // Add data and apply styles
            int row = 2;
            foreach (var entry in fileVerifications)
            {
                worksheet.Cells[row, 1].Value = entry.CatchNumber;
                worksheet.Cells[row, 2].Value = entry.SeriesName;

                // Determine status value and apply style
                string statusValue;
                var cellStyle = worksheet.Cells[row, 3].Style;
                cellStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Set pattern type before color

                if (entry.Status == 1)
                {
                    statusValue = "Correct";
                    cellStyle.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen); // Correct
                }
                else if (entry.Status == 0)
                {
                    statusValue = "Not Verified";
                    cellStyle.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Not Verified
                }
                else if (entry.Status == 2)
                {
                    statusValue = "Incorrect";
                    cellStyle.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral); // Incorrect
                }
                else
                {
                    statusValue = "Unknown"; // Handle unexpected status values
                    cellStyle.Fill.BackgroundColor.SetColor(System.Drawing.Color.White); // Default color for unknown
                }

                worksheet.Cells[row, 3].Value = statusValue;

                // Only include VerifiedAt time for Correct or Incorrect status
                if (entry.Status == 1 || entry.Status == 2)
                {
                    worksheet.Cells[row, 5].Value = entry.VerifiedAt != DateTime.MinValue ? entry.VerifiedAt.ToString("yyyy-MM-dd HH:mm:ss") : "N/A";
                }
                else
                {
                    worksheet.Cells[row, 5].Value = "N/A"; // Set as N/A for Not Verified or Unknown
                }

                worksheet.Cells[row, 4].Value = entry.VerifiedByName;

                row++;
            }

            // Convert to byte array and return as file
            var fileContent = package.GetAsByteArray();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StatusReport.xlsx");
        }

        //GetAccountSendingEnabledRequest filterd catch numbers 
        [AllowAnonymous]
        [HttpGet("catch-numbers")]
        public async Task<IActionResult> GetCatchNumbers([FromQuery] int programId, [FromQuery] string statusFilter)
        {
            if (string.IsNullOrEmpty(statusFilter) || !new[] { "verified", "notverified", "verifiedincorrect", "all" }.Contains(statusFilter.ToLower()))
            {
                return BadRequest("Invalid status filter provided. Valid values are 'verified', 'notverified', 'verifiedincorrect', or 'all'.");
            }

            // Step 1: Retrieve all catch numbers with their status in one query
            var catchNumberStatuses = await _dbContext.PDFfiles
                .Where(pdf => pdf.ProgramId == programId)
                .GroupBy(pdf => pdf.CatchNumber)
                .Select(g => new
                {
                    CatchNumber = g.Key,
                    AllSeriesVerified = g.All(pdf => pdf.Status == 1),
                    AnySeriesVerifiedIncorrect = g.Any(pdf => pdf.Status == 2),
                    AllSeriesNotVerified = g.All(pdf => pdf.Status != 1 && pdf.Status != 2)
                })
                .ToListAsync();

            // Step 2: Filter catch numbers based on the statusFilter
            var filteredCatchNumbers = catchNumberStatuses.Where(cn =>
            {
                switch (statusFilter.ToLower())
                {
                    case "verified":
                        return cn.AllSeriesVerified;
                    case "notverified":
                        return cn.AllSeriesNotVerified;
                    case "verifiedincorrect":
                        return cn.AnySeriesVerifiedIncorrect;
                    case "all":
                        return true;
                    default:
                        return false;
                }
            })
            .Select(cn => new
            {
                CatchNumber = cn.CatchNumber,
                IsFullyVerified = cn.AllSeriesVerified,
                HasVerifiedIncorrect = cn.AnySeriesVerifiedIncorrect,
                IsNotVerified = cn.AllSeriesNotVerified
            })
            .OrderBy(cn => cn.CatchNumber)  // Order by catch number in ascending order
            .ToList();

            return Ok(filteredCatchNumbers);
        }



        //StatusCodeHttpResult count responce  
        [AllowAnonymous]
        [HttpGet("GetStatusCount/{programId}")]
        public async Task<ActionResult<StatusCountResponse>> GetStatusCount(int programId)
        {
            // Initialize the response
            var response = new StatusCountResponse();

            try
            {
                // 1. Get the total count of papers for the specified programId
                response.TotalPapers = await _dbContext.Papers
                    .Where(p => p.ProgrammeID == programId)
                    .CountAsync();

                // 2. Get the total count of PDF files grouped by unique CatchNumber for the programId
                var pdfFilesGroupedByCatch = await _dbContext.PDFfiles
                    .Where(p => p.ProgramId == programId)
                    .GroupBy(p => p.CatchNumber)
                    .Select(g => new
                    {
                        CatchNumber = g.Key,
                        TotalFilesInCatch = g.Count(),
                        VerifiedCount = g.Count(p => p.Status == 1),    // Correct (Status 1)
                        NotVerifiedCount = g.Count(p => p.Status == 0), // Not Verified (Status 0)
                        WrongCount = g.Count(p => p.Status == 2)        // Wrong (Status 3)
                    }).ToListAsync();

                // 3. Sum up the counts for PDF statuses
                response.TotalFiles = pdfFilesGroupedByCatch.Sum(x => x.TotalFilesInCatch); // Total number of files
                response.TotalCatchNumbers = pdfFilesGroupedByCatch.Count(); // Total unique catch numbers
                response.VerifiedCount = pdfFilesGroupedByCatch.Count(x => x.VerifiedCount == x.TotalFilesInCatch); // Only if all are verified
                response.NotVerifiedCount = pdfFilesGroupedByCatch.Count(x => x.VerifiedCount == 0 && x.WrongCount == 0); // None are verified or wrong
                response.WrongCount = pdfFilesGroupedByCatch.Count(x => x.WrongCount == x.TotalFilesInCatch); // All are wrong

                // Return the response with status 200 OK
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 error
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet("GetAnswersbyPage")]
        public async Task<ActionResult<List<Answers>>> GetAnswersbyPage(string CatchNumber, int PageNumber, int ProgramId, int SetId)
        {
            var answers = await _dbContext.AnswersKeys.Where(u => u.CatchNumber == CatchNumber && u.ProgID == ProgramId && u.PageNumber == PageNumber && u.SetID == SetId).ToListAsync();
            if (answers == null)
            {
                return NotFound();
            }
            return Ok(answers);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            if (_dbContext.PDFfiles == null)
            {
                return NotFound("Database context is null.");
            }

            // Find the PDF file entry in the database
            var pdffile = await _dbContext.PDFfiles.FindAsync(id);
            if (pdffile == null)
            {
                return NotFound("File not found in database.");
            }

            // Get the file path from the database (relative path stored)
            var relativeFilePath = pdffile.FilePath;

            // Convert the relative path to an absolute path
            var absoluteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeFilePath);

            // Check if the file exists
            if (System.IO.File.Exists(absoluteFilePath))
            {
                // Delete the physical file from the folder
                System.IO.File.Delete(absoluteFilePath);
            }
            else
            {
                return NotFound("File not found on the server.");
            }

            // Remove the file entry from the database
            _dbContext.PDFfiles.Remove(pdffile);
            await _dbContext.SaveChangesAsync();

            return Ok("File deleted successfully.");
        }

    }

    public class StatusUpdateRequest
    {
        public int Status { get; set; } // true for Verified, false for Wrong
        public int ProgramId { get; set; }
        public string CatchNumber { get; set; }
    }
    public class InputPdf
    {
        public IFormFile File { get; set; }
    }
    public class StatusCountResponse
    {
        public int TotalPapers { get; set; }         // Total papers from the papers table
        public int TotalFiles { get; set; }          // Total number of individual PDF files
        public int TotalCatchNumbers { get; set; }   // Total number of unique CatchNumbers
        public int VerifiedCount { get; set; }       // CatchNumbers where all PDFs are Verified
        public int NotVerifiedCount { get; set; }    // CatchNumbers where some PDFs are not Verified
        public int WrongCount { get; set; }          // CatchNumbers where all PDFs are Wrong
    }


}
