using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class Programme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProgrammeID { get; set; }

        public string? ProgrammeName { get; set; }

        [Required]
        public int GroupID { get; set; }

        [Required]
        public int  SessionID{ get; set; }

        [Required]
        public int TypeID { get; set; }
    }
}
