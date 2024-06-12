using System.ComponentModel.DataAnnotations;
using App.Attributes;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace App.Dtos.UserDtos;

public class UserRegisterDto
{

    [Required]
    [MinLength(8, ErrorMessage = "Username must be at least 8 characters long.")]
    [MaxLength(20, ErrorMessage = "Username cannot be more than 20 characters long.")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and digits.")]
    public required string UserName { get; set; } = string.Empty;
    
    [Required]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters long.")]
    [MaxLength(100, ErrorMessage = "Password cannot be more than 100 characters long.")]
    [ValidPassword(ErrorMessage = "Password must be at least 12 characters long, including at least two uppercase letters, two lowercase letters, two numbers, and two special characters.")]    
    [ComparePasswords(nameof(Password2), ErrorMessage = "Passwords do not match.")]
    [CompareUserName(nameof(UserName), ErrorMessage = "Passwords can not be same as UserName.")]
    public required string Password1 { get; set; } = string.Empty;
    
    [Required]
    public required string Password2 { get; set; } = string.Empty;
}