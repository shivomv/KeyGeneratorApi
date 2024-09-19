using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyGenerator.Models
{
    public class FileVerification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CatchNumber { get; set; }

        public int ProgramId { get; set; }

        public int PageNumber { get; set; }

        public bool IsCorrect { get; set; }

        public int VerifiedBy { get; set; }

        public string SeriesName { get; set; }

        public DateTime VerifiedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
