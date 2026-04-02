using CoworkerHub.Application.MappingProfiles;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Services;
using CoworkerHub.Application.Validations;
using CoworkerHub.Infrastructure.Persistens;
using FluentValidation;

namespace CoworkerHub.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddAuthorization();
            services.AddInfrastructureServices(configuration);
            services.AddValidatorsFromAssemblyContaining<CreateWorkspaceValidator>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(AppMappingProfile));
            services.AddEndpointsApiExplorer();
            services.SwaggerExtensions(configuration);
            services.AddScoped<IDeskService, DeskService>();
        }
    }
}
