using App.Dtos.UserDtos;
using App.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService? userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Executing GetAll method");
                var users = await _userService.GetAllAsync();
                _logger.LogInformation("GetAll method executed successfully");
                return Ok(users);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in UserGetAll method");
                return Forbid("You do not have permission to Get All User information.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing GetAll Users method");
                return Conflict("An error occurred while retrieving users. Please try again later.");
            }
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<UserIncludeDataDto>> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Executing GetById method with id: {Id}", id);
                var user = await _userService.GetByIdAsync(id);
                _logger.LogInformation("GetById method executed successfully for id: {Id}", id);
                return Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in User GetById method");
                return Forbid("You do not have permission to Get User information.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing GetById method with id: {Id}", id);
                return Conflict("An error occurred while retrieving the user. Please try again later.");
            }
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                _logger.LogInformation("Executing Register method for userName: {UserName}", userRegisterDto.UserName);
                await _userService.RegisterAsync(userRegisterDto);
                _logger.LogInformation("User registered successfully with userName: {UserName}", userRegisterDto.UserName);
                return Ok(new Response { Message = "User created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with userName: {UserName}", userRegisterDto.UserName);
                return Conflict("An error occurred while registering the user. Please try again later.");
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                _logger.LogInformation("Executing Login method for userName: {UserName}", userLoginDto.UserName);
                var token = await _userService.LoginAsync(userLoginDto);
                _logger.LogInformation("User logged in successfully with userName: {UserName}", userLoginDto.UserName);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user with userName: {UserName}", userLoginDto.UserName);
                return Conflict("An error occurred while logging in. Please check your credentials and try again.");
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            try
            {
                _logger.LogInformation("Executing Delete User method with id: {Id}", id);
                await _userService.DeleteUserAsync(id);
                _logger.LogInformation("User deleted successfully with id: {Id}", id);
                return Ok(new Response { Message = "Account has been deleted successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in Delete method");
                return Forbid("You do not have permission to Delete User.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with id: {Id}", id);
                return Conflict("An error occurred while deleting the user. Please try again later.");
            }
        }
    }
}
