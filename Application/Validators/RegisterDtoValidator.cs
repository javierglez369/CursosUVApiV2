using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Must(p => p.Any(char.IsUpper))
                .WithMessage("La contraseña debe contener al menos una mayúscula.")
            .Must(p => p.Any(char.IsDigit))
                .WithMessage("La contraseña debe contener al menos un número.");

        RuleFor(r => r.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.")
            .Matches(@"^[a-záéíóúñA-ZÁÉÍÓÚÑ\s]+$")
                .WithMessage("El nombre solo puede contener letras y espacios.");

        RuleFor(r => r.Apellidos)
            .NotEmpty().WithMessage("Los apellidos son obligatorios.")
            .MaximumLength(150).WithMessage("Los apellidos no pueden exceder 150 caracteres.")
            .Matches(@"^[a-záéíóúñA-ZÁÉÍÓÚÑ\s]+$")
                .WithMessage("Los apellidos solo pueden contener letras y espacios.");
    }
}