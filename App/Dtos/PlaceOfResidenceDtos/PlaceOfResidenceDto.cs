using System.ComponentModel.DataAnnotations;

namespace App.Dtos;

public class PlaceOfResidenceDto
{
    public Guid Id { get; set; }

    [Required]
    public required string City { get; set; } = string.Empty;

    [Required]
    public required string Street { get; set; } = string.Empty;
    [Required]
    public required int HouseNumber { get; set; }
    public int ApartmentNumber { get; set; }
}