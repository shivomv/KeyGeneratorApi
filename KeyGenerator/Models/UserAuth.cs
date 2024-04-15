using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace KeyGenerator.Models
{
    public class UserAuth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserAuthID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        public bool AutoGenPass { get; set; }
    }
}
