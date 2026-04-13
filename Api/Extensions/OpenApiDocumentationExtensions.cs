using Scalar.AspNetCore;

namespace Api.Extensions;

public static class OpenApiDocumentationExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi(options =>
        {
            // Configurar autenticación JWT Bearer
            options.UseJwtBearerAuthentication();

            // Configurar información de la API
            options.ConfigureApiInfo(configuration);
        });

        return services;
    }


    public static IEndpointRouteBuilder UseOpenApiDocumentation(this IEndpointRouteBuilder app, IConfiguration configuration)
    {
        var openApiEnabled = configuration.GetValue<bool>("OpenApi:Enabled", true);

        if (!openApiEnabled)
            return app;

        // Mapear el endpoint OpenAPI
        app.MapOpenApi();

        // Mapear Scalar como interfaz de documentación
        app.MapScalarApiReference(options =>
        {
            options.Title = configuration.GetValue<string>("Swagger:Title") ?? "Web API Documentation";
            options.DefaultFonts = false;
        });

        // Redireccionar la raíz a la documentación
        var redirectPath = configuration.GetValue<string>("OpenApi:RedirectPath") ?? "/scalar/v1";
        app.MapGet("/", () => Results.Redirect(redirectPath))
            .ExcludeFromDescription();

        return app;
    }
}