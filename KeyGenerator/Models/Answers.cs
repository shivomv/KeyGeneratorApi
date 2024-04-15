using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class Answers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SerialNumber { get; set; }

        [Required]
        public int PageNumber { get; set; }

        [Required]
        public int QuestionNumber { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Answer { get; set; }

        [Required]
        public int ProgID { get; set; }

        [Required]
        public string CatchNumber { get; set; }

        [Required]
        public int PaperID { get; set; }

        [Required]
        public int SetID { get; set; }

    }
}
