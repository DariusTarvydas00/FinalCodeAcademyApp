using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.IServices;
using DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace App.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration), " cannot be null or not initialized");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), " cannot be null or not initialized");
        }

        public async Task<User?> CreateUserAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation("Creating user with username: {Username}", username);
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
                
                var user = new User
                {
                    UserName = username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = "User"
                };

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user with username: {Username}", username);
                throw;
            }
        }

        public void CreatePasswordHash(string? password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            try
            {
                using var hmac = new HMACSHA512();
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating password hash");
                throw;
            }
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            try
            {
                using var hmac = new HMACSHA512(storedSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return StructuralComparisons.StructuralEqualityComparer.Equals(computedHash, storedHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying password");
                throw;
            }
        }

        public string GenerateJwtToken(string username, Guid userId, string? role)
        {
            try
            {
                _logger.LogInformation("Generating JWT token for username: {Username}", username);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, role ?? throw new ArgumentNullException(nameof(role)))
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtSecretKey()));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

                var token = new JwtSecurityToken(
                    GetJwtIssuer(),
                    GetJwtAudience(),
                    claims,
                    expires: DateTime.Now.AddSeconds(GetJwtExpirationSeconds()),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating JWT token for username: {Username}", username);
                throw;
            }
        }

        private string GetJwtSecretKey()
        {
            try
            {
                var key = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(key))
                {
                    throw new InvalidOperationException("Jwt secret key is missing in configuration.");
                }
                return key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Jwt secret key");
                throw;
            }
        }

        private string GetJwtIssuer()
        {
            try
            {
                var issuer = _configuration["Jwt:Issuer"];
                if (string.IsNullOrEmpty(issuer))
                {
                    throw new InvalidOperationException("Jwt issuer is missing in configuration.");
                }
                return issuer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Jwt issuer");
                throw;
            }
        }

        private string GetJwtAudience()
        {
            try
            {
                var audience = _configuration["Jwt:Audience"];
                if (string.IsNullOrEmpty(audience))
                {
                    throw new InvalidOperationException("Jwt audience is missing in configuration.");
                }
                return audience;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Jwt audience");
                throw;
            }
        }

        private int GetJwtExpirationSeconds()
        {
            try
            {
                var expirationSeconds = _configuration["Jwt:ExpirationSeconds"];
                if (string.IsNullOrEmpty(expirationSeconds) || !int.TryParse(expirationSeconds, out var seconds))
                {
                    throw new InvalidOperationException("Jwt expiration seconds are missing or invalid in configuration.");
                }
                return seconds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Jwt expiration seconds");
                throw;
            }
        }
    }
}
