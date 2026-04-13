namespace Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            // Política de producción: solo orígenes conocidos
            options.AddPolicy("CorsPolicy", policy =>
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials());   // necesario si usas cookies de refresh token

            // Política amplia: solo para Development (no agregar en Producción)
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());
            // ⚠ AllowAnyOrigin() es incompatible con AllowCredentials()
            // Por eso "AllowAll" NO tiene AllowCredentials — solo para pruebas locales
        });

        return services;
    }

    public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        // ✅ CORS va ANTES de Authentication y Authorization
        if (environment.IsDevelopment())
            app.UseCors("AllowAll");
        else
            app.UseCors("CorsPolicy");

        return app;
    }
}