using DataAccess.Entities;

namespace DataAccess.IRepositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<List<User>> GetAllAsync();
    Task<User?> GetByUserNameAsync(string username);
    Task CreateAsync(User user);
    Task DeleteAsync(User user);
}