using System.Text.Json.Serialization;
using App.Dtos.FileDtos;

namespace App.Dtos.PersonInformationDtos;

public class PersonInformationIncludeDataDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public DateTime Birthday { get; set; }
    public int PersonalCode { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public PlaceOfResidenceDto PlaceOfResidenceDto { get; set; }
    public ProfilePhotoDto ProfilePhotoDto { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }
}