using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorID { get; set; }

        public string Error { get; set; }

        public string Message { get; set; }

        public string OccuranceSpace{ get; set; }

        public DateTime LoggedAt { get; set; } = DateTime.Now;
    }
}
