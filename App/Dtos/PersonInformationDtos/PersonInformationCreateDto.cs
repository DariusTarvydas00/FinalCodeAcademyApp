using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using App.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace App.Dtos.PersonInformationDtos;

public class PersonInformationCreateDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    [ValidName]
    public required string FirstName { get; set; }
    
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    [ValidName]
    public required string LastName { get; set; }
    
    [Required]
    [ValidGender]
    public required string Gender { get; set; }
    
    [Required]
    [ValidBirthday]
    public required string Birthday { get; set; }
    
    [Required]
    [ValidPersonalCode]
    public required string PersonalCode { get; set; }
    
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    [ValidEmail]
    public required string Email { get; set; }
    
    [Required]
    public required PlaceOfResidenceCreateDto PlaceOfResidenceDto { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }
}