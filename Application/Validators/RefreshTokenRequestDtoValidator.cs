using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators;

public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty().WithMessage("El refresh token es obligatorio.")
            .NotNull().WithMessage("El refresh token no puede ser nulo.")
            .MinimumLength(10).WithMessage("El refresh token tiene un formato inválido.");
    }
}