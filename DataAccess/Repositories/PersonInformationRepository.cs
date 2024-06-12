using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PersonInformationRepository : IPersonInformationRepository
    {
        private readonly MainDbContext _mainDbContext;

        public PersonInformationRepository(MainDbContext mainDbContext)
        {
            _mainDbContext = mainDbContext ?? throw new ArgumentNullException(nameof(mainDbContext));
        }

        public async Task<List<PersonInformation?>> GetAllAsync(Guid userId)
        {
            return await _mainDbContext.PersonInformations.Where(information => information != null && information.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<PersonInformation?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _mainDbContext.PersonInformations
                .Include(pi => pi.PlaceOfResidence)
                .Include(information => information.ProfilePhoto)
                .FirstOrDefaultAsync(information => 
                    information != null && information.Id.Equals(id) && information.UserId.Equals(userId));
        }

        public async Task AddAsync(PersonInformation entity,Guid userId)
        {
            entity.UserId = userId;
            await _mainDbContext.PersonInformations.AddAsync(entity);
            await _mainDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PersonInformation entity,Guid userId)
        {
            var personInformation = await _mainDbContext.PersonInformations.FirstOrDefaultAsync(information =>
                information != null && information.Id == entity.Id && information.UserId == userId);

            if (personInformation == null)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this information.");
            }
            _mainDbContext.PersonInformations.Update(personInformation);
            await _mainDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id,Guid userId)
        {
            var entity = await _mainDbContext.PersonInformations
                .FirstOrDefaultAsync(information => 
                    information != null 
                    && information.UserId.Equals(userId) 
                    && information.Id.Equals(id));
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }

            _mainDbContext.PersonInformations.Remove(entity);
            await _mainDbContext.SaveChangesAsync();
        }
    }
}