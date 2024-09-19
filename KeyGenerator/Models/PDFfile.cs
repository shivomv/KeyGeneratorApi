using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyGenerator.Models
{
    public class PDFfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string CatchNumber { get; set; }

        [MaxLength(1)]
        public string SeriesName { get; set; }

        public int ProgramId { get; set; }

        public int Status { get; set; }

        public int VerifiedBy { get; set; }
        public DateTime VerifiedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

    }
}
