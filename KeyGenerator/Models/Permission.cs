using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int ModuleID { get; set; }

        public bool Can_Add { get; set; } = false;

        public bool Can_Delete { get; set; } = false;

        public bool Can_Update { get; set; } = false;

        public bool Can_View { get; set; } = false;
    }
}
