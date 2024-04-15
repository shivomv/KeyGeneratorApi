using KeyGenerator.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace KeyGenerator.Models
{
    [Index(nameof(EmailAddress), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        [NameCheck]
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; }

        public string Designation { get; set; }

        public bool Status { get; set; } = true;

        // New property for profile picture path
        public string ProfilePicturePath { get; set; }
    }
}
