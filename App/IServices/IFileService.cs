using Microsoft.AspNetCore.Http;

namespace App.IServices
{
    public interface IFileService
    {
        Task UploadFile(IFormFile file, Guid userId, Guid personInformationId);
        Task Delete(Guid id, Guid userId);
        Task<(Stream FileStream, string FileName, string ContentType)> Download(Guid id, Guid userId);
    }
}