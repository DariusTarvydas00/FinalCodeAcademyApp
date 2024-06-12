using App.Dtos.PersonInformationDtos;
using App.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helper;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class PersonInformationController : ControllerBase
    {
        private readonly IPersonInformationService _personInformationService;
        private readonly ILogger<PersonInformationController> _logger;

        public PersonInformationController(IPersonInformationService personInformationService, ILogger<PersonInformationController> logger)
        {
            _personInformationService = personInformationService ?? throw new ArgumentNullException(nameof(personInformationService), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonInformationDto>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Executing GetAll method");
                var userId = ClaimsHelper.GetUserId(User);
                var items = await _personInformationService.GetAllAsync(userId);
                _logger.LogInformation("GetAll method executed successfully");
                return Ok(items);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in GetAll method");
                return Forbid("You do not have permission to use GetAll Method.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing GetAll method");
                return StatusCode(500, "An error occurred while retrieving the information. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonInformationIncludeDataDto>> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Executing GetById method with id: {Id}", id);
                var userId = ClaimsHelper.GetUserId(User);
                var item = await _personInformationService.GetByIdAsync(id, userId);
                _logger.LogInformation("GetById method executed successfully for id: {Id}", id);
                return Ok(item);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in GetById method");
                return Forbid("You do not have permission to use GetById Method.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing GetById method with id: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the information. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add(PersonInformationCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Executing Add method");
                var userId = ClaimsHelper.GetUserId(User);
                await _personInformationService.AddAsync(dto, userId);
                _logger.LogInformation("Person Information added successfully");
                return Ok(new Response { Message = "Person Information created successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in Add method");
                return Forbid("You do not have permission to Add Person information.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Adding Person Information with id: {Id}", dto.UserId);
                return StatusCode(500, "An error occurred while Adding the information.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(PersonInformationUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Executing Update method with id: {Id}", dto.Id);
                var userId = ClaimsHelper.GetUserId(User);
                _logger.LogInformation("User ID extracted: {UserId}", userId);
                await _personInformationService.UpdateAsync(dto, userId);
                _logger.LogInformation("Person Information updated successfully with id: {Id}", dto.Id);
                return Ok(new Response { Message = "Person Information updated successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in Update method");
                return Forbid("You do not have permission to update this information.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating Person Information with id: {Id}", dto.Id);
                return StatusCode(500, "An error occurred while updating the information.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Executing Delete method with id: {Id}", id);
                var userId = ClaimsHelper.GetUserId(User);
                await _personInformationService.DeleteAsync(id, userId);
                _logger.LogInformation("Person Information deleted successfully with id: {Id}", id);
                return Ok(new Response { Message = "Person Information deleted successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in Delete method");
                return Forbid("You do not have permission to delete this information.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting Person Information with id: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the information.");
            }
        }
    }
}
