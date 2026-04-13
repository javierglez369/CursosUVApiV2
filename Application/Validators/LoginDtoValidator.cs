using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

        RuleFor(l => l.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}