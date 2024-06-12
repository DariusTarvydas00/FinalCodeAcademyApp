using System.Text.Json.Serialization;

namespace App.Dtos.PersonInformationDtos;

public class PersonInformationDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public string Birthday { get; set; }
    public string PersonalCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }
}