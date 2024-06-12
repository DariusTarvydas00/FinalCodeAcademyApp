using System.ComponentModel.DataAnnotations;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace App.Dtos.UserDtos;

public class UserLoginDto
{

    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and digits.")]
    public required string UserName { get; set; }

    [Required]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters long.")]
    [MaxLength(100, ErrorMessage = "Password cannot be more than 100 characters long.")]
    [RegularExpression("^(?=(.*[a-z]){2})(?=(.*[A-Z]){2})(?=(.*\\d){2})(?=(.*[@$!%*?&]){2})[A-Za-z\\d@$!%*?&]{12,}$", ErrorMessage = "Password must be at least 12 characters long, including at least two uppercase letters, two lowercase letters, two numbers, and two special characters.")]
    public required string Password { get; set; }

}