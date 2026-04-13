using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public class UpdateCategoriaDtoValidator : AbstractValidator<UpdateCategoriaDto>
{
    public UpdateCategoriaDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
                .WithMessage("El nombre es obligatorio.")
            .MinimumLength(3)
                .WithMessage("El nombre debe tener al menos 3 caracteres.")
            .MaximumLength(100)
                .WithMessage("El nombre no puede superar los 100 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ0-9\s\-\.]+$")
                .WithMessage("El nombre solo puede contener letras, números, espacios, guiones y puntos.");

        RuleFor(x => x.Descripcion)
            .NotEmpty()
                .WithMessage("La descripción es obligatoria.")
            .MinimumLength(10)
                .WithMessage("La descripción debe tener al menos 10 caracteres.")
            .MaximumLength(500)
                .WithMessage("La descripción no puede superar los 500 caracteres.");
    }
}