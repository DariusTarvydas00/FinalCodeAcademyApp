using Microsoft.AspNetCore.Mvc;
using App.IServices;
using Microsoft.AspNetCore.Authorization;
using WebApi.Helper;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, Guid personInformationId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            try
            {
                _logger.LogInformation("Executing UploadFile method for user: {UserId}", userId);
                if (file.Length == 0)
                {
                    _logger.LogWarning("File not selected or empty for user: {UserId}", userId);
                    return BadRequest("File not selected or empty");
                }

                await _fileService.UploadFile(file, userId, personInformationId);
                _logger.LogInformation("File uploaded successfully for user: {UserId}", userId);
                return Ok(new { message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading file for user: {UserId}", userId);
                return Conflict("An error occurred while uploading the file. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var userId = ClaimsHelper.GetUserId(User);
            try
            {
                _logger.LogInformation("Executing DeleteFile method for file: {FileId} and user: {UserId}", id, userId);
                await _fileService.Delete(id, userId);
                _logger.LogInformation("File deleted successfully for file: {FileId} and user: {UserId}", id, userId);
                return Ok(new { message = "File deleted successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in DeleteFile method for file: {FileId} and user: {UserId}", id, userId);
                return Forbid("You do not have permission to delete this file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting file: {FileId} for user: {UserId}", id, userId);
                return Conflict("An error occurred while deleting the file. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var userId = ClaimsHelper.GetUserId(User);
            try
            {
                _logger.LogInformation("Executing DownloadFile method for file: {FileId} and user: {UserId}", id, userId);
                var (fileStream, fileName, contentType) = await _fileService.Download(id, userId);
      
                _logger.LogInformation("File downloaded successfully for file: {FileId} and user: {UserId}", id, userId);
                return File(fileStream, contentType, fileName);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized access in DownloadFile method for file: {FileId} and user: {UserId}", id, userId);
                return Forbid("You do not have permission to download this file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while downloading file: {FileId} for user: {UserId}", id, userId);
                return Conflict("An error occurred while downloading the file. Please try again later.");
            }
        }
    }
}
