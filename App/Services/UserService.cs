using System.Security.Authentication;
using App.Dtos.UserDtos;
using App.IServices;
using AutoMapper;
using DataAccess.IRepositories;
using Microsoft.Extensions.Logging;

namespace App.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IJwtService jwtService, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository), " cannot be null or not initialized");
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService), " cannot be null or not initialized");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
        }

        public async Task<UserIncludeDataDto> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting user by id: {Id}", id);
                var user = await _userRepository.GetByIdAsync(id);
                var userDto = _mapper.Map<UserIncludeDataDto>(user);
                _logger.LogInformation("Successfully retrieved user by id: {Id}", id);
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by id: {Id}", id);
                throw;
            }
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all users");
                var users = await _userRepository.GetAllAsync();
                var userDtos = _mapper.Map<List<UserDto>>(users);
                _logger.LogInformation("Successfully retrieved all users");
                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                throw;
            }
        }

        public async Task RegisterAsync(UserRegisterDto userRegisterDto)
        {
            try
            {
                _logger.LogInformation("Registering user with username: {Username}", userRegisterDto.UserName);

                var user = await _userRepository.GetByUserNameAsync(userRegisterDto.UserName);
                if (user != null)
                {
                    _logger.LogWarning("User with username: {Username} already exists", userRegisterDto.UserName);
                    throw new InvalidDataException("User with such username already exists");
                }

                var newUser = await _jwtService.CreateUserAsync(userRegisterDto.UserName, userRegisterDto.Password1);
                if (newUser != null) 
                {
                    await _userRepository.CreateAsync(newUser);
                    _logger.LogInformation("Successfully registered user with username: {Username}", userRegisterDto.UserName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with username: {Username}", userRegisterDto.UserName);
                throw;
            }
        }

        public async Task<string> LoginAsync(UserLoginDto userLoginDto)
        {
            try
            {
                _logger.LogInformation("User login attempt with username: {Username}", userLoginDto.UserName);
                var user = await _userRepository.GetByUserNameAsync(userLoginDto.UserName);
                if (user == null)
                {
                    _logger.LogWarning("Invalid username: {Username}", userLoginDto.UserName);
                    throw new AuthenticationException("Invalid username or password!");
                }

                if (!_jwtService.VerifyPassword(userLoginDto.Password, user.PasswordHash, user.PasswordSalt))
                {
                    _logger.LogWarning("Invalid password for username: {Username}", userLoginDto.UserName);
                    throw new AuthenticationException("Invalid username or password!");
                }
                var token = _jwtService.GenerateJwtToken(userLoginDto.UserName, user.Id, user.Role);
                _logger.LogInformation("User successfully logged in with username: {Username}", userLoginDto.UserName);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login attempt with username: {Username}", userLoginDto.UserName);
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting user with id: {Id}", id);
                var user = await _userRepository.GetByIdAsync(id);
                if (user != null)
                {
                    await _userRepository.DeleteAsync(user);
                    _logger.LogInformation("Successfully deleted user with id: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("User with id: {Id} not found", id);
                    throw new InvalidOperationException("User not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with id: {Id}", id);
                throw;
            }
        }
    }
}
