using DataAccess.Entities;

namespace DataAccess.IRepositories;

public interface IFileRepository
{
    Task<ProfilePhoto?> Download(Guid id, Guid userId);
    Task Delete(Guid id, Guid userId);
    Task UploadFile(ProfilePhoto file, Guid userId);
}