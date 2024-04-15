using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeyGenerator.Models
{
    public class EventLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }

        public string Event { get; set; }

        public string Category { get; set; }

        public int EventTriggeredBy { get; set; }

        public DateTime LoggedAT { get; set; } = DateTime.Now;

    }
}
