using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class Paper
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaperID { get; set; }

        [Required]
        public int ProgrammeID { get; set; } // Coming from programme

        [Required]
        public string PaperName { get; set; } // Sociology of religion

        [Required]
        public string CatchNumber { get; set; } // text field

        public string? PaperCode { get; set; }

        public int? CourseID { get; set; } // coming from Master


        public string? ExamType { get; set; } // ex. 2nd Sem etc

        public int? SubjectID { get; set; }

        public string? PaperNumber { get; set; }

        public DateTime ExamDate { get; set; } 

        public int? NumberofQuestion { get; set; }

        public int? BookletSize { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        [Required]
        public int CreatedByID { get; set; }

        public bool MasterUploaded { get; set; } = false;

        public bool KeyGenerated { get; set; } = false;

        

    }
}
