using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class SetofStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int ProgConfigID { get; set; }

        [Required]
        public string steps { get; set; } //5,6 //10,11

        [Required]
        public int StepID { get; set; } //1 //2
    }
}
