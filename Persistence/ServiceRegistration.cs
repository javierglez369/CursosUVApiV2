using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;
using Persistence.Services;

namespace Persistence;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
        ));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // AuthService requiere ApplicationDbContext, IConfiguration, UserManager e IUnitOfWork
        services.AddScoped<IAuthService>(provider =>
            new AuthService(
                provider.GetRequiredService<ApplicationDbContext>(),
                configuration,
                provider.GetRequiredService<UserManager<ApplicationUser>>(),
                provider.GetRequiredService<IUnitOfWork>()));

        // ── Identity (solo el core — sin JWT, sin middleware HTTP) ─────────
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();



        return services;
    }

}
