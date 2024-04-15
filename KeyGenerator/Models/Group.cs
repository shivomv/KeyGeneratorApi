using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using KeyGenerator.Validators;

namespace KeyGenerator.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupID { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public bool status { get; set; }
    }
}
