using System.Security.Authentication;
using App.Dtos.UserDtos;
using App.IServices;
using App.Services;
using AutoMapper;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppTest
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _mapperMock = new Mock<IMapper>();
            Mock<ILogger<UserService>> loggerMock = new();
            _userService = new UserService(
                _userRepositoryMock.Object,
                _jwtServiceMock.Object,
                _mapperMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUserDto()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "TestUser",
                PasswordHash = [],
                PasswordSalt = [],
                Role = null
            };
            var userDto = new UserIncludeDataDto { Id = userId, UserName = "TestUser" };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<UserIncludeDataDto>(user)).Returns(userDto);

            var result = await _userService.GetByIdAsync(userId);

            Assert.Equal(userDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _userService.GetByIdAsync(userId));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUserDtos()
        {
            var users = new List<User> { new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "TestUser",
                    PasswordHash = [],
                    PasswordSalt = [],
                    Role = null
                }
            };
            var userDtos = new List<UserDto> { new UserDto { Id = Guid.NewGuid(), UserName = "TestUser" } };

            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);
            _mapperMock.Setup(mapper => mapper.Map<List<UserDto>>(users)).Returns(userDtos);

            var result = await _userService.GetAllAsync();

            Assert.Equal(userDtos, result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrowException_WhenErrorOccurs()
        {
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _userService.GetAllAsync());
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser()
        {
            var userRegisterDto = new UserRegisterDto
            {
                UserName = "TestUser",
                Password1 = "ValidPassword123!",
                Password2 = "ValidPassword123!"
            };
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                PasswordHash = [],
                PasswordSalt = [],
                Role = null
            };

            _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(userRegisterDto.UserName)).ReturnsAsync((User?)null);
            _jwtServiceMock.Setup(service => service.CreateUserAsync(userRegisterDto.UserName, userRegisterDto.Password1)).ReturnsAsync(newUser);

            await _userService.RegisterAsync(userRegisterDto);

            _userRepositoryMock.Verify(repo => repo.CreateAsync(newUser), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenPasswordsDoNotMatch()
        {
            var userRegisterDto = new UserRegisterDto
            {
                UserName = "TestUser",
                Password1 = "ValidPassword123!",
                Password2 = "DifferentPassword123!"
            };

            await Assert.ThrowsAsync<InvalidDataException>(() => _userService.RegisterAsync(userRegisterDto));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            var userRegisterDto = new UserRegisterDto
            {
                UserName = "TestUser",
                Password1 = "ValidPassword123!",
                Password2 = "ValidPassword123!"
            };
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                PasswordHash = [],
                PasswordSalt = [],
                Role = null
            };

            _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(userRegisterDto.UserName)).ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<InvalidDataException>(() => _userService.RegisterAsync(userRegisterDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnJwtToken()
        {
            var userLoginDto = new UserLoginDto
            {
                UserName = "TestUser",
                Password = "ValidPassword123!"
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                PasswordHash = [],
                PasswordSalt = [],
                Role = "User"
            };
            const string token = "jwt_token";

            _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(userLoginDto.UserName)).ReturnsAsync(user);
            _jwtServiceMock.Setup(service => service.VerifyPassword(userLoginDto.Password, user.PasswordHash, user.PasswordSalt)).Returns(true);
            _jwtServiceMock.Setup(service => service.GenerateJwtToken(userLoginDto.UserName, user.Id, user.Role)).Returns(token);

            var result = await _userService.LoginAsync(userLoginDto);

            Assert.Equal(token, result);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowAuthenticationException_WhenUserNotFound()
        {
            var userLoginDto = new UserLoginDto
            {
                UserName = "NonExistentUser",
                Password = "ValidPassword123!"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(userLoginDto.UserName)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<AuthenticationException>(() => _userService.LoginAsync(userLoginDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowAuthenticationException_WhenPasswordIsIncorrect()
        {
            var userLoginDto = new UserLoginDto
            {
                UserName = "TestUser",
                Password = "IncorrectPassword"
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                PasswordHash = [],
                PasswordSalt = [],
                Role = null
            };

            _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(userLoginDto.UserName)).ReturnsAsync(user);
            _jwtServiceMock.Setup(service => service.VerifyPassword(userLoginDto.Password, user.PasswordHash, user.PasswordSalt)).Returns(false);

            await Assert.ThrowsAsync<AuthenticationException>(() => _userService.LoginAsync(userLoginDto));
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "TestUser",
                PasswordHash = [],
                PasswordSalt = [],
                Role = null
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            await _userService.DeleteUserAsync(userId);

            _userRepositoryMock.Verify(repo => repo.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowException_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.DeleteUserAsync(userId));
        }
    }
}
