using DataAccess.Entities;

namespace DataAccess.IRepositories;

public interface IPersonInformationRepository
{
    Task<List<PersonInformation?>> GetAllAsync(Guid userId);
    Task<PersonInformation?> GetByIdAsync(Guid id,Guid userId);
    Task AddAsync(PersonInformation entity,Guid userId);
    Task UpdateAsync(PersonInformation entity,Guid userId);
    Task DeleteAsync(Guid id,Guid userId);
}