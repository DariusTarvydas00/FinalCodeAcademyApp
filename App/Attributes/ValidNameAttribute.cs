using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Attributes;

public class ValidNameAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "Invalid First Name or Last Name";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var n = value as string;

        if (string.IsNullOrEmpty(n))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        var regex = new Regex("^[a-zA-Z]*$");
        if (!regex.IsMatch(n))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        return ValidationResult.Success;
    }
}