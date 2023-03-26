using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServicesExtensions
{
   public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
            IConfiguration config)
            {
                //Connection with SQLite
                services.AddDbContext<DataContext>(optionsAction: opt =>
                {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
                });
                //Connection with Angular app
                services.AddCors();
                services.AddScoped<ITokenService, TokenService>();

                return services;

            }
}
