using App.Dtos.PersonInformationDtos;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace App.Dtos.UserDtos;

public class UserIncludeDataDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<PersonInformationIncludeDataDto>? PersonInformationDtos { get; set; } = new();
}