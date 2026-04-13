using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Api.Extensions;

public static class OpenApiTransformersExtensions
{

    public static OpenApiOptions UseJwtBearerAuthentication(this OpenApiOptions options)
    {
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Ingrese el token JWT en el encabezado: Bearer {token}"
        };

        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, scheme);
            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security = [new() { { new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme), [] } }];
            }
            return Task.CompletedTask;
        });

        return options;
    }


    public static OpenApiOptions ConfigureApiInfo(this OpenApiOptions options, IConfiguration configuration)
    {
        var apiInfo = configuration.GetSection("ApiInfo");

        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info.Title = apiInfo["Title"] ?? "Web API";
            document.Info.Version = apiInfo["Version"] ?? "v1";
            document.Info.Description = apiInfo["Description"] ?? "API desarrollada con .NET 10";

            // Configurar contacto
            document.Info.Contact = new()
            {
                Name = apiInfo["ContactName"] ?? "Equipo de Desarrollo",
                Email = apiInfo["ContactEmail"] ?? "contacto@correo.com",
                Url = new Uri(apiInfo["ContactUrl"] ?? "https://sitioweb.com")
            };

            // Configurar licencia
            document.Info.License = new()
            {
                Name = apiInfo["LicenseName"] ?? "MIT License",
                Url = new Uri(apiInfo["LicenseUrl"] ?? "https://opensource.org/licenses/MIT")
            };

            return Task.CompletedTask;
        });

        return options;
    }
}