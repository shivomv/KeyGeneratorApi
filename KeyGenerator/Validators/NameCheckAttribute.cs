using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KeyGenerator.Validators
{
    public class NameCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var name = (string)value;
            string pattern = @"^[a-zA-Z\s]+$";
            Regex regex = new Regex(pattern);
            bool iscorrect = regex.IsMatch(name);
            if (!iscorrect)
            {
                return new ValidationResult("Name should not contain any special character ");
            }

            return ValidationResult.Success;
        }
    }
}
