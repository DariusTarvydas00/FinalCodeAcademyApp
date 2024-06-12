using System.ComponentModel.DataAnnotations;

namespace App.Attributes;

public class ValidGenderAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var gender = value as string;
        if (gender != "Male" && gender != "Female")
        {
            return new ValidationResult("Gender must be either 'Male' or 'Female'.");
        }

        return ValidationResult.Success;
    }
}