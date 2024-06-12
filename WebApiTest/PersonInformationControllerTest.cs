using System.Security.Claims;
using App.Dtos.PersonInformationDtos;
using App.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Controllers;
using WebApi.Models;

namespace WebApiTest
{
    public class PersonInformationControllerTests
    {
        private readonly Mock<IPersonInformationService> _personInformationServiceMock;
        private readonly PersonInformationController _controller;

        public PersonInformationControllerTests()
        {
            _personInformationServiceMock = new Mock<IPersonInformationService>();
            Mock<ILogger<PersonInformationController>> loggerMock = new();
            _controller = new PersonInformationController(_personInformationServiceMock.Object, loggerMock.Object);
        }

        private void SetUserRole(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Role, role),
                new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAllForUser_Returns_PersonInformationList()
        {
            var persons = new List<PersonInformationDto?> { new() { Id = Guid.NewGuid(), FirstName = "Test" } };
            _personInformationServiceMock.Setup(service => service.GetAllAsync(It.IsAny<Guid>())).ReturnsAsync(persons);
            SetUserRole("User");

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(persons, okResult.Value);
        }

        [Fact]
        public async Task GetAllWithoutRole_Returns_ForbiddenException()
        {
            _personInformationServiceMock.Setup(service => service.GetAllAsync(It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("");

            var result = await _controller.GetAll();

            Assert.IsType<ForbidResult>(result.Result);
        }
        
        [Fact]
        public async Task GetAllForAdmin_Returns_ForbiddenException()
        {
            _personInformationServiceMock.Setup(service => service.GetAllAsync(It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("Admin");

            var result = await _controller.GetAll();

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetByIdForUser_Returns_PersonInformation()
        {
            var personId = Guid.NewGuid();
            var person = new PersonInformationIncludeDataDto() { Id = personId, FirstName = "Test" };
            _personInformationServiceMock.Setup(service => service.GetByIdAsync(personId, It.IsAny<Guid>())).ReturnsAsync(person);
            SetUserRole("User");

            var result = await _controller.GetById(personId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(person, okResult.Value);
        }
        
        [Fact]
        public async Task GetByIdWithoutRole_Returns_ForbiddenException()
        {
            var personId = Guid.NewGuid();
            _personInformationServiceMock.Setup(service => service.GetByIdAsync(personId, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("");

            var result = await _controller.GetById(personId);

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetByIdForAdmin_Returns_ForbiddenException()
        {
            var personId = Guid.NewGuid();
            _personInformationServiceMock.Setup(service => service.GetByIdAsync(personId, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("Admin");

            var result = await _controller.GetById(personId);

            Assert.IsType<ForbidResult>(result.Result);
        }

        // [Fact]
        // public async Task AddPersonInformationForUser_Returns_SuccessfullyCreatedMessage()
        // {
        //     var personCreateDto = new PersonInformationCreateDto { FirstName = "Test", LastName = "User", GenderDto = "Male", Birthday = DateTime.Now, PersonalCode = 1234567890, PhoneNumber = 1234567890, Email = "test@example.com" };
        //     _personInformationServiceMock.Setup(service => service.AddAsync(personCreateDto, It.IsAny<Guid>())).Returns(Task.CompletedTask);
        //     SetUserRole("User");
        //
        //     var result = await _controller.Add(personCreateDto);
        //
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     Assert.Equal(200, okResult.StatusCode);
        //     Assert.Equal(new Response { Message = "Person Information created successfully" }.Message, (okResult.Value as Response)?.Message);
        // }
        //
        // [Fact]
        // public async Task AddPersonInformationWithoutRole_Returns_ForbiddenException()
        // {
        //     var personCreateDto = new PersonInformationCreateDto { FirstName = "Test", LastName = "User", GenderDto = "Male", Birthday = DateTime.Now, PersonalCode = 1234567890, PhoneNumber = 1234567890, Email = "test@example.com" };
        //     _personInformationServiceMock.Setup(service => service.AddAsync(personCreateDto, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
        //     SetUserRole("");
        //
        //     var result = await _controller.Add(personCreateDto);
        //
        //     Assert.IsType<ForbidResult>(result);
        // }
        //
        // [Fact]
        // public async Task AddPersonInformationForAdmin_Returns_ForbiddenException()
        // {
        //     var personCreateDto = new PersonInformationCreateDto { FirstName = "Test", LastName = "User", GenderDto = "Male", Birthday = DateTime.Now, PersonalCode = 1234567890, PhoneNumber = 1234567890, Email = "test@example.com" };
        //     _personInformationServiceMock.Setup(service => service.AddAsync(personCreateDto, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
        //     SetUserRole("Admin");
        //
        //     var result = await _controller.Add(personCreateDto);
        //
        //     Assert.IsType<ForbidResult>(result);
        // }
        //
        // [Fact]
        // public async Task UpdatePersonInformationForUser_Returns_SuccessfullyUpdatedMessage()
        // {
        //     var personUpdateDto = new PersonInformationUpdateDto { Id = Guid.NewGuid(), FirstName = "Updated", LastName = "User", GenderDto = "Male", Birthday = DateTime.Now, PersonalCode = 1234567890, PhoneNumber = 1234567890, Email = "updated@example.com" };
        //     _personInformationServiceMock.Setup(service => service.UpdateAsync(personUpdateDto, It.IsAny<Guid>())).Returns(Task.CompletedTask);
        //     SetUserRole("User");
        //
        //     var result = await _controller.Update(personUpdateDto);
        //
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     Assert.Equal(200, okResult.StatusCode);
        //     Assert.Equal(new Response { Message = "Person Information updated successfully" }.Message, (okResult.Value as Response)?.Message);
        // }
        //
        // [Fact]
        // public async Task UpdatePersonInformationWithoutRole_Returns_ForbiddenException()
        // {
        //     var personUpdateDto = new PersonInformationUpdateDto { Id = Guid.NewGuid(), FirstName = "Updated", LastName = "User", GenderDto = "Male", Birthday = DateTime.Now, PersonalCode = 1234567890, PhoneNumber = 1234567890, Email = "updated@example.com" };
        //     _personInformationServiceMock.Setup(service => service.UpdateAsync(personUpdateDto, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
        //     SetUserRole("");
        //
        //     var result = await _controller.Update(personUpdateDto);
        //
        //     Assert.IsType<ForbidResult>(result);
        // }
        //
        // [Fact]
        // public async Task UpdatePersonInformationForAdmin_Returns_ForbiddenException()
        // {
        //     var personUpdateDto = new PersonInformationUpdateDto { Id = Guid.NewGuid(), FirstName = "Updated", LastName = "User", GenderDto = "Male", Birthday = DateTime.Now, PersonalCode = 1234567890, PhoneNumber = 1234567890, Email = "updated@example.com" };
        //     _personInformationServiceMock.Setup(service => service.UpdateAsync(personUpdateDto, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
        //     SetUserRole("Admin");
        //
        //     var result = await _controller.Update(personUpdateDto);
        //
        //     Assert.IsType<ForbidResult>(result);
        // }

        [Fact]
        public async Task DeletePersonInformationForUser_Returns_SuccessfullyDeletedMessage()
        {
            var personId = Guid.NewGuid();
            _personInformationServiceMock.Setup(service => service.DeleteAsync(personId, It.IsAny<Guid>())).Returns(Task.CompletedTask);
            SetUserRole("User");

            var result = await _controller.Delete(personId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(new Response { Message = "Person Information deleted successfully" }.Message, (okResult.Value as Response)?.Message);
        }

        [Fact]
        public async Task DeletePersonInformationForAdmin_Returns_ForbiddenException()
        {
            var personId = Guid.NewGuid();
            _personInformationServiceMock.Setup(service => service.DeleteAsync(personId, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("Admin");

            var result = await _controller.Delete(personId);

            Assert.IsType<ForbidResult>(result);
        }
        
        [Fact]
        public async Task DeletePersonInformationWithoutRole_Returns_ForbiddenException()
        {
            var personId = Guid.NewGuid();
            _personInformationServiceMock.Setup(service => service.DeleteAsync(personId, It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
            SetUserRole("");

            var result = await _controller.Delete(personId);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
