﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyGenerator.Models
{
    public class BookletPdfData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookletPDFID { get; set; }

        [Required]
        public int PaperID { get; set; }

        public string BookletData { get; set; }
        public int UploadedBy { get; set; }

        public DateTime UploadedDateTime { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
