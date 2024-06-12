using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly MainDbContext _mainDbContext;
        private readonly ILogger<FileRepository> _logger;

        public FileRepository(MainDbContext mainDbContext, ILogger<FileRepository> logger)
        {
            _mainDbContext = mainDbContext ?? throw new ArgumentNullException(nameof(mainDbContext), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
        }

        public async Task UploadFile(ProfilePhoto file, Guid userId)
        {
            try
            {
                _logger.LogInformation("Uploading file: {FileName} for user: {UserId}", file.FileName, file.UserId);
                file.UserId = userId;
                await _mainDbContext.ProfilePhotos.AddAsync(file);
                await _mainDbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully uploaded file: {FileName} for user: {UserId}", file.FileName, file.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading file: {FileName} for user: {UserId}", file.FileName, file.UserId);
                throw;
            }
        }

        public async Task Delete(Guid id, Guid userId)
        {
            try
            {
                _logger.LogInformation("Deleting file with id: {Id} for user: {UserId}", id, userId);
                var file = await _mainDbContext.ProfilePhotos.FirstOrDefaultAsync(note => note.Id == id && note.UserId == userId);
                if (file != null)
                {
                    _mainDbContext.ProfilePhotos.Remove(file);
                    await _mainDbContext.SaveChangesAsync();
                    _logger.LogInformation("Successfully deleted file with id: {Id} for user: {UserId}", id, userId);
                }
                else
                {
                    _logger.LogWarning("File with id: {Id} not found for user: {UserId}", id, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting file with id: {Id} for user: {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ProfilePhoto?> Download(Guid id, Guid userId)
        {
            try
            {
                _logger.LogInformation("Downloading file with id: {Id} for user: {UserId}", id, userId);
                var file = await _mainDbContext.ProfilePhotos.FirstOrDefaultAsync(note => note.Id == id && note.UserId == userId);
                if (file == null)
                {
                    _logger.LogWarning("File with id: {Id} not found for user: {UserId}", id, userId);
                }
                else
                {
                    _logger.LogInformation("Successfully found file with id: {Id} for user: {UserId}", id, userId);
                }
                return file;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading file with id: {Id} for user: {UserId}", id, userId);
                throw;
            }
        }
    }
}
