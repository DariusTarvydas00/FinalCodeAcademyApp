using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Attributes;

public class ValidPersonalCodeAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "Invlaid Personal Code";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var pc = value as string;

        if (string.IsNullOrEmpty(pc))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        var regex = new Regex("^[0-9][0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[1-2][0-9]|3[0-1])[0-9]{4}$");
        if (!regex.IsMatch(pc))
        {
            return new ValidationResult(DefaultErrorMessage);
        }

        return ValidationResult.Success;
    }
}