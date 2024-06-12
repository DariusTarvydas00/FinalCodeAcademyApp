using App.Dtos.PersonInformationDtos;
using App.IServices;
using AutoMapper;
using DataAccess.Entities;
using DataAccess.IRepositories;

namespace App.Services
{
    public class PersonInformationService : IPersonInformationService
    {
        private readonly IPersonInformationRepository _personInformationRepository;
        private readonly IMapper _mapper;

        public PersonInformationService(IPersonInformationRepository personInformationRepository, IMapper mapper)
        {
            _personInformationRepository = personInformationRepository ?? throw new ArgumentNullException(nameof(personInformationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<PersonInformationDto?>> GetAllAsync(Guid userId)
        {
            var entities = await _personInformationRepository.GetAllAsync(userId);
            return _mapper.Map<List<PersonInformationDto?>>(entities);
        }

        public async Task<PersonInformationIncludeDataDto> GetByIdAsync(Guid id, Guid userId)
        {
            var entity = await _personInformationRepository.GetByIdAsync(id, userId);
            if (entity == null || entity.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to view this information.");
            }
            return _mapper.Map<PersonInformationIncludeDataDto>(entity);
        }

        public async Task AddAsync(PersonInformationCreateDto personInformationDto, Guid userId)
        {
            var entity = _mapper.Map<PersonInformation>(personInformationDto);
            await _personInformationRepository.AddAsync(entity, userId);
        }

        public async Task UpdateAsync(PersonInformationUpdateDto personInformationUpdateDto, Guid userId)
        {
            var existingEntity = await _personInformationRepository.GetByIdAsync(personInformationUpdateDto.Id, userId);
            if (existingEntity == null || existingEntity.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this information.");
            }
            var entity = _mapper.Map<PersonInformation>(personInformationUpdateDto);
            await _personInformationRepository.UpdateAsync(entity, userId);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var existingEntity = await _personInformationRepository.GetByIdAsync(id, userId);
            if (existingEntity == null || existingEntity.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this information.");
            }
            await _personInformationRepository.DeleteAsync(id, userId);
        }
    }
}
