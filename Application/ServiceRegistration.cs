using Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {

        //Escanear los perfiles de Automapper
        services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

        services.AddSingleton<CategoriaFactoria>();

        return services;
    }
}
