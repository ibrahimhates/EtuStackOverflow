using AskForEtu.Core.Hash;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services;
using AskForEtu.Repository.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using Microsoft.EntityFrameworkCore;

namespace EtuStackOverflow.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureDbContext(this IServiceCollection services,
            IConfiguration configuration) 
            => services.AddDbContext<AskForEtuDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("mySql");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
        }

        public static void ConfigureRepos(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMajorRepository, MajorRepository>();
            services.AddScoped<IFacultRepository, FacultyRepository>();
        }

        public static void ConfigureResponsibility(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }
    }
}
