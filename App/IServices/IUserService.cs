using App.Dtos.UserDtos;

namespace App.IServices;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserIncludeDataDto> GetByIdAsync(Guid id);
    Task RegisterAsync(UserRegisterDto userRegisterDto);
    Task<string> LoginAsync(UserLoginDto userLoginDto);
    Task DeleteUserAsync(Guid id);
}