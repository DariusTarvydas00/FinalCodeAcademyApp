using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Attributes;

public class ValidEmailAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "Invalid Email";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var e = value as string;

        if (string.IsNullOrEmpty(e))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        var regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
        if (!regex.IsMatch(e))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        return ValidationResult.Success;
    }
}