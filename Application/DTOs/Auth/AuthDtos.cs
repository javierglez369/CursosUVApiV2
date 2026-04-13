namespace Application.DTOs.Auth;

// ── DTOs de entrada (required para validación) ────────────────────────────
public record RegisterDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Nombre { get; init; }
    public required string Apellidos { get; init; }
}

public record LoginDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public record RefreshTokenRequestDto
{
    public required string RefreshToken { get; init; }
}

// ── DTOs de salida (posicionales — solo lectura) ───────────────────────────

public record UserDto(
    string Id,
    string Email,
    string NombreCompleto,
    IList<string> Roles);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);

public record RefreshTokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);

public record UserRegistrationResponseDto(
    string UserId,
    string Email,
    string NombreCompleto);