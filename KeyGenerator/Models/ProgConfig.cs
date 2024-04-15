using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class ProgConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProgConfigID { get; set; }

        [Required]
        public int ProgID { get; set; }

        [Required]
        public int Sets { get; set; }

        [Required]
        public string SetOrder { get; set; }

        [Required]
        public int NumberofQuestions { get; set; }

        [Required]
        public int BookletSize { get; set; }

        [Required]
        public int NumberofJumblingSteps { get; set; }

        public int SetofStepsID { get; set; }

    }
}
