using AskForEtu.Core.Dto.Mail;
using AskForEtu.Core.Hash;
using AskForEtu.Core.JwtGenerator;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Queue;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.JwtGenerator;
using AskForEtu.Repository.Services;
using AskForEtu.Repository.Services.Queue;
using AskForEtu.Repository.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using EmailSenderService;
using EmailSenderService.EmailService;
using EtuStackOverflow.OptionsSetup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
        }

        public static void ConfigureRepos(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMajorRepository, MajorRepository>();
            services.AddScoped<IFacultRepository, FacultyRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
        }

        public static void ConfigureResponsibility(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        public static void ConfigureJwtBearer(this IServiceCollection services,IConfiguration configuration)
        {
            //services.ConfigureOptions<JwtBearerOptionsSetup>();

            var _jwtOptions = configuration.GetSection("JwtOptions");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _jwtOptions["Issuer"],
                        ValidAudience = _jwtOptions["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_jwtOptions["SecretKey"]))
                    };
                });

            services.ConfigureOptions<JwtOptionsSetup>();
            services.AddScoped<IJwtProvider, JwtProvider>();
        }

        public static void ConfigureEmailService(this IServiceCollection services)
        {
            services.AddHostedService<SenderBgService>();

            services.ConfigureOptions<EmailOptionsSetup>();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddSingleton<ITaskQueue<EmailSendTemplate>, EmailTaskQueue>();
        }
    }
}
