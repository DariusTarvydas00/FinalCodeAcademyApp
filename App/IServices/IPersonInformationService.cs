using App.Dtos.PersonInformationDtos;

namespace App.IServices;

public interface IPersonInformationService
{
    Task<List<PersonInformationDto?>> GetAllAsync(Guid userId);
    Task<PersonInformationIncludeDataDto> GetByIdAsync(Guid id, Guid userId);
    Task AddAsync(PersonInformationCreateDto entity, Guid userId);
    Task UpdateAsync(PersonInformationUpdateDto entity, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}