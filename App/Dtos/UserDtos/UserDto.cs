// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace App.Dtos.UserDtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}