using DataAccess.Entities;

namespace App.IServices;

public interface IJwtService
{
    Task<User?> CreateUserAsync(string username, string password);
    bool VerifyPassword(string password, byte[] userPasswordHash, byte[] userPasswordSalt);
    string GenerateJwtToken(string username, Guid userId, string? role);
}