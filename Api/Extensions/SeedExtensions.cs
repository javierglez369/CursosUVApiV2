using Microsoft.AspNetCore.Identity;

namespace Api.Extensions;

public static class SeedExtensions
{
    /// <summary>
    /// Inicializa los roles por defecto en la base de datos.
    /// </summary>
    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = ["Administrador", "Instructor", "Estudiante"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}