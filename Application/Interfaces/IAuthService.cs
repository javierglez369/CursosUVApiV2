using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAuthService
{
    // ── REGISTRO ───────────────────────────────────────────────────────────

    /// <summary>
    /// Registra un nuevo usuario (ApplicationUser + perfil de Estudiante).
    /// Encapsula toda la lógica:
    /// - Validación de email duplicado
    /// - Creación de ApplicationUser con hash de contraseña
    /// - Creación de perfil Estudiante vinculado
    /// - Asignación de rol "Estudiante" por defecto
    /// </summary>
    Task<UserRegistrationResponseDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default);

    // ── AUTENTICACIÓN ──────────────────────────────────────────────────────

    /// <summary>
    /// Autentica un usuario (email + contraseña) y genera tokens JWT y refresh.
    /// Encapsula toda la lógica:
    /// - Validación de credenciales
    /// - Generación de JWT con claims
    /// - Generación de refresh token
    /// </summary>
    Task<AuthResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Renueva el JWT usando un refresh token válido (token rotation).
    /// Encapsula toda la lógica:
    /// - Validación del refresh token
    /// - Revocación del token usado
    /// - Generación de nuevo JWT + refresh token
    /// </summary>
    Task<RefreshTokenResponseDto> RefreshAsync(
        RefreshTokenRequestDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera un JWT firmado con los claims del usuario y sus roles.
    /// </summary>
    string GenerateJwtToken(ApplicationUser user, IList<string> roles);

    /// <summary>
    /// Genera un refresh token, lo persiste en BD y retorna el string del token.
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(ApplicationUser user);

    /// <summary>
    /// Valida que el refresh token exista, no esté revocado y no haya expirado.
    /// Retorna null si no es válido.
    /// </summary>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);

    /// <summary>
    /// Marca el refresh token como revocado (logout / token rotation).
    /// </summary>
    Task RevokeRefreshTokenAsync(string token);
}