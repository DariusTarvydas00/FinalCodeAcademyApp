using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Attributes;

public class ValidPasswordAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "Password must be at least 12 characters long, including at least two uppercase letters, two lowercase letters, two numbers, and two special characters.";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var p = value as string;

        if (string.IsNullOrEmpty(p))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        var regex = new Regex("^(?=(.*[a-z]){2})(?=(.*[A-Z]){2})(?=(.*\\d){2})(?=(.*[@$!%*?&]){2})[A-Za-z\\d@$!%*?&]{12,}$");
        if (!regex.IsMatch(p))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        return ValidationResult.Success;
    }
}