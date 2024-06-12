using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Attributes;

public class ValidBirthdayAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "Invalid Birthday";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var b = value as string;

        if (string.IsNullOrEmpty(b))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        var regex = new Regex(@"^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d$");
        if (!regex.IsMatch(b))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        return ValidationResult.Success;
    }
}