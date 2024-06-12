using App.Dtos.FileDtos;
using App.IServices;
using AutoMapper;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace App.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly IPersonInformationRepository _personInformationRepository;
        private readonly ILogger<FileService> _logger;

        public FileService(IFileRepository fileRepository, IMapper mapper, ILogger<FileService> logger, IPersonInformationRepository personInformationRepository)
        {
            _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository), " cannot be null or not initialized");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
            _personInformationRepository = personInformationRepository;
        }

        public async Task UploadFile(IFormFile file, Guid userId, Guid personInformationId)
        {
            try
            {
                _logger.LogInformation("Uploading file: {FileName} for user: {UserId}", file.FileName, userId);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("File is empty or not selected for user: {UserId}", userId);
                    throw new ArgumentException("File is empty or not selected", nameof(file));
                }
                
                // Verify the existence of PersonInformationId
                var personInfo = await _personInformationRepository.GetByIdAsync(personInformationId,userId);
                if (personInfo == null)
                {
                    _logger.LogWarning("PersonInformation with Id: {PersonInformationId} does not exist", personInformationId);
                    throw new ArgumentException("Invalid PersonInformationId", nameof(personInformationId));
                }

                var fileEntity = _mapper.Map<ProfilePhoto>(new ProfilePhotoCreateDto
                {
                    PersonInformationId = personInfo.Id,
                    FileName = file.FileName,
                    FilePath = Path.Combine("uploads", file.FileName)
                });
                

                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Directory does not exist"));

                await using (var stream = file.OpenReadStream())
                {
                    var image = await Image.LoadAsync(stream);
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(200, 200),
                        Mode = ResizeMode.Crop
                    }));

                    await using var output = new FileStream(path, FileMode.Create);
                    await image.SaveAsync(output, new JpegEncoder());
                }


                await _fileRepository.UploadFile(fileEntity, userId);
                _logger.LogInformation("Successfully uploaded file: {FileName} for user: {UserId}", file.FileName, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading file: {FileName} for user: {UserId}", file?.FileName, userId);
                throw;
            }
        }

        public async Task Delete(Guid id, Guid userId)
        {
            try
            {
                _logger.LogInformation("Deleting file with id: {Id} for user: {UserId}", id, userId);

                var existingEntity = await _fileRepository.Download(id, userId);
                if (existingEntity == null || existingEntity.UserId != userId)
                {
                    _logger.LogWarning("Unauthorized access attempt to delete file with id: {Id} by user: {UserId}", id, userId);
                    throw new UnauthorizedAccessException("You do not have permission to delete this file.");
                }

                await _fileRepository.Delete(id, userId);

                var path = Path.Combine(Directory.GetCurrentDirectory(), existingEntity.FilePath);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                _logger.LogInformation("Successfully deleted file with id: {Id} for user: {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting file with id: {Id} for user: {UserId}", id, userId);
                throw;
            }
        }

        public async Task<(Stream FileStream, string FileName, string ContentType)> Download(Guid id, Guid userId)
        {
            try
            {
                _logger.LogInformation("Downloading file with id: {Id} for user: {UserId}", id, userId);

                var fileEntity = await _fileRepository.Download(id, userId);
                if (fileEntity == null || fileEntity.UserId != userId)
                {
                    _logger.LogWarning("Unauthorized access attempt to download file with id: {Id} by user: {UserId}", id, userId);
                    throw new UnauthorizedAccessException("You do not have permission to download this file.");
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), fileEntity.FilePath);
                if (!File.Exists(path))
                {
                    _logger.LogWarning("File with id: {Id} not found on server for user: {UserId}", id, userId);
                    throw new FileNotFoundException("File not found on the server.");
                }

                var memoryStream = new MemoryStream();
                await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memoryStream);
                }
                memoryStream.Position = 0;

                _logger.LogInformation("Successfully downloaded file with id: {Id} for user: {UserId}", id, userId);

                return (memoryStream, fileEntity.FileName, "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while downloading file with id: {Id} for user: {UserId}", id, userId);
                throw;
            }
        }
    }
}
