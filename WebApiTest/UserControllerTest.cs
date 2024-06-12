using System.Security.Claims;
using App.Dtos.UserDtos;
using App.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Controllers;
using WebApi.Models;

namespace WebApiTest
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            Mock<ILogger<UserController>> loggerMock = new();
            _controller = new UserController(_userServiceMock.Object, loggerMock.Object);
        }
        
        private void SetUserRole(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.Role, role)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAllForAdmin_Returns_Users()
        {
            var users = new List<UserDto> { new() { Id = Guid.NewGuid(), UserName = "TestUser" } };
            _userServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(users);
            SetUserRole("Admin");

            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(users, okResult.Value);
        }
        
        [Fact]
        public async Task GetAllWithoutRole_Returns_ForbiddenException()
        {
            _userServiceMock.Setup(service => service.GetAllAsync()).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("");
            
            var result = await _controller.GetAll();
            
            Assert.IsType<ForbidResult>(result.Result);
        }
        
        [Fact]
        public async Task GetAllForUser_Returns_ForbiddenException()
        {
            _userServiceMock.Setup(service => service.GetAllAsync()).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("User");
            
            var result = await _controller.GetAll();
            
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetByIdForAdmin_Returns_User()
        {
            var userId = "0b2e05c1-7b27-44c6-8e37-8b7d8e4f7b8e";
            var id = Guid.Parse(userId);
            var user = new UserIncludeDataDto { Id = Guid.Parse(userId), UserName = "TestUser" };
            
            _userServiceMock.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(user);
            SetUserRole("Admin");

            var result = await _controller.GetById(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(user, okResult.Value);
        }
        
        [Fact]
        public async Task GetByIdForUser_Returns_ForbiddenException()
        {
            var id = Guid.NewGuid();
            _userServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("User");
            
            var result = await _controller.GetById(id);

            Assert.IsType<ForbidResult>(result.Result);
        }
        
        [Fact]
        public async Task GetByIdWithoutRole_Returns_ForbiddenException()
        {
            var id = Guid.NewGuid();
            _userServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("");
            
            var result = await _controller.GetById(id);

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task Register_Returns_SuccessfullyRegisteredMessage()
        {
            var username = "TestUser";
            var password = "password1234";
            var userRegisterDto = new UserRegisterDto { UserName = username, Password1 = password, Password2 = password };
            _userServiceMock.Setup(service => service.RegisterAsync(userRegisterDto)).Returns(Task.CompletedTask);
            
            var result = await _controller.Register(userRegisterDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(new Response { Message = "User created successfully" }.Message, (okResult.Value as Response)?.Message);
        }
        
        [Fact]
        public async Task Login_Returns_JWT()
        {
            var username = "TestUser";
            var password = "password1234";
            var token = "testToken";
            var userLoginDto = new UserLoginDto() { UserName = username, Password = password };
            _userServiceMock.Setup(service => service.LoginAsync(userLoginDto)).ReturnsAsync(token);

            var result = await _controller.Login(userLoginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(token, okResult.Value);
        }

        [Fact]
        public async Task DeleteUserForAdmin_Returns_SuccessfullyDeletedMessage()
        {
            var userId = "0b2e05c1-7b27-44c6-8e37-8b7d8e4f7b8e";
            var id = Guid.Parse(userId);
            _userServiceMock.Setup(service => service.DeleteUserAsync(id)).Returns(Task.CompletedTask);
            SetUserRole("Admin");

            var result = await _controller.DeleteUser(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(new Response { Message = "Account has been deleted successfully" }.Message, (okResult.Value as Response)?.Message);
        }
        
        [Fact]
        public async Task DeleteUserForUser_Returns_ForbiddenException()
        {
            var userId = "0b2e05c1-7b27-44c6-8e37-8b7d8e4f7b8e";
            var id = Guid.Parse(userId);
            _userServiceMock.Setup(service => service.DeleteUserAsync(id)).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("User");

            var result = await _controller.DeleteUser(id);

            Assert.IsType<ForbidResult>(result);
        }
        
        [Fact]
        public async Task DeleteUserWithoutRole_Returns_ForbiddenException()
        {
            var userId = "0b2e05c1-7b27-44c6-8e37-8b7d8e4f7b8e";
            var id = Guid.Parse(userId);
            _userServiceMock.Setup(service => service.DeleteUserAsync(id)).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("");

            var result = await _controller.DeleteUser(id);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
