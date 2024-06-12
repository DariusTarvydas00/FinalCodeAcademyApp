using App.IServices;
using App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace App.Extensions;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService,JwtService>();
        services.AddAutoMapper(typeof(ServiceRegistration).Assembly);
        
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPersonInformationService, PersonInformationService>();
        services.AddScoped<IFileService, FileService>();
    }
}