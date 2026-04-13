using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Attributes;

/// <summary>
/// Filtro de acción genérico que valida el parámetro de tipo <typeparamref name="TDto"/>
/// usando el <see cref="IValidator{T}"/> registrado en el contenedor DI.
/// Si la validación falla, lanza <see cref="ValidationException"/> que el
/// <see cref="API.Middleware.ExceptionMiddleware"/> convierte en 400 con lista de errores.
/// </summary>
/// <typeparam name="TDto">Tipo del DTO a validar.</typeparam>
public class ValidateAttribute<TDto> : ActionFilterAttribute
    where TDto : class
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // 1. Buscar el argumento del tipo TDto en los parámetros del action
        var parametro = context.ActionArguments.Values
            .OfType<TDto>()
            .FirstOrDefault();

        if (parametro is null)
        {
            // No hay parámetro del tipo esperado — continuar sin validar
            await next();
            return;
        }

        // 2. Obtener el validator del DI
        var validator = context.HttpContext.RequestServices
            .GetService<IValidator<TDto>>();

        if (validator is null)
        {
            // No hay validator registrado — continuar (no es error, es opcional)
            await next();
            return;
        }

        // 3. Ejecutar validación
        var resultado = await validator.ValidateAsync(parametro);

        if (!resultado.IsValid)
        {
            // 4. Recopilar todos los mensajes de error
            List<string> errores = resultado.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            // 5. Lanzar ValidationException — el ExceptionMiddleware la captura
            throw new Domain.Exceptions.ValidationException(errores);
        }

        await next();
    }
}