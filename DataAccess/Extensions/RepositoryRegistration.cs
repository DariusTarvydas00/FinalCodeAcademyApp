using DataAccess.IRepositories;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class RepositoryRegistration
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<MainDbContext>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPersonInformationRepository, PersonInformationRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
    }
}