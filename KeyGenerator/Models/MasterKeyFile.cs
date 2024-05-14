using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyGenerator.Models
{
    public class MasterKeyFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MasterKeyFileID { get; set; }

        [Required]
        public int PaperID { get; set; }

        public string MasterKeyFileData { get; set; }

        public int UploadedBy { get; set; }

        public DateTime UploadedDateTime { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
