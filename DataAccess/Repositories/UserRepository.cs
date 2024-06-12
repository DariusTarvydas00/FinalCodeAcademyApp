using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MainDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(MainDbContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching user by id: {Id}", id);
                var user = await _context.Users
                    .Include(u => u.PersonInformations) // Include PersonInformations
                    .ThenInclude(pi => pi.PlaceOfResidence) // Include PlaceOfResidence within PersonInformations
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("User with id: {Id} not found", id);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user by id: {Id}", id);
                throw;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var users = await _context.Set<User>().ToListAsync();
                _logger.LogInformation("Successfully fetched all users");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all users");
                throw;
            }
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            try
            {
                _logger.LogInformation("Fetching user by username: {Username}", username);
                var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                {
                    _logger.LogWarning("User with username: {Username} not found", username);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user by username: {Username}", username);
                throw;
            }
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                _logger.LogInformation("Creating user with username: {Username}", user.UserName);
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully created user with username: {Username}", user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user with username: {Username}", user.UserName);
                throw;
            }
        }

        public async Task DeleteAsync(User user)
        {
            try
            {
                _logger.LogInformation("Deleting user with id: {Id}", user.Id);
                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted user with id: {Id}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with id: {Id}", user.Id);
                throw;
            }
        }
    }
}
