using System.ComponentModel.DataAnnotations;

namespace App.Attributes;

public class CompareUserNameAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public CompareUserNameAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = value as string;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
        if (property == null)
            throw new ArgumentException("Property with this name not found");

        var comparisonValue = property.GetValue(validationContext.ObjectInstance) as string;

        if (string.Equals(currentValue, comparisonValue))
            return new ValidationResult(ErrorMessage ?? "Passwords can not be same as UserName");

        return ValidationResult.Success;
    }
}