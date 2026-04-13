using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {

        //Escanear los perfiles de Automapper
        services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
    
        services.AddScoped<ICategoriaService, CategoriaService>();

        services.AddValidatorsFromAssemblyContaining<CreateCategoriaDtoValidator>();

        return services;
    }

}
