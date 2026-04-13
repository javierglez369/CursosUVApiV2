using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Persistence.Services;

public class AuthService(
    ApplicationDbContext context,
    IConfiguration configuration,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork uow) : IAuthService
{
    // ─── REGISTRO DE USUARIO ──────────────────────────────────────────────

    /// <summary>
    /// Registra un nuevo usuario completando toda la lógica de negocio:
    /// 1. Validación de email duplicado
    /// 2. Creación de ApplicationUser (identidad del sistema)
    /// 3. Creación de perfil Estudiante (entidad de dominio)
    /// 4. Asignación del rol "Estudiante"
    /// 
    /// Si algo falla, lanza AppException y no deja datos inconsistentes.
    /// </summary>
    public async Task<UserRegistrationResponseDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        // 1. VALIDACIÓN: Verificar que el email no exista
        if (await userManager.FindByEmailAsync(dto.Email) is not null)
            throw new AppException(
                "El email ya está registrado",
                HttpStatusCode.BadRequest);

        var nombreCompleto = $"{dto.Nombre} {dto.Apellidos}";

        // 2. CREAR ApplicationUser (identidad de seguridad)
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            NombreCompleto = nombreCompleto
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errores = result.Errors
                .Select(e => e.Description)
                .ToList();
            throw new AppException(
                "Error al registrar usuario",
                errores,
                HttpStatusCode.BadRequest);
        }

        // 3. CREAR perfil de Estudiante (entidad de negocio)
        // Vincula la identidad de seguridad (ApplicationUser) con el perfil de negocio
        var estudiante = new Estudiante
        {
            Nombre = dto.Nombre,
            Apellidos = dto.Apellidos,
            Email = dto.Email,
            UserId = user.Id  // Foreign key → ApplicationUser
        };

        await uow.Repository<Estudiante>().AddAsync(estudiante, cancellationToken);
        await uow.SaveChangesAsync();

        // 4. ASIGNAR ROL por defecto
        await userManager.AddToRoleAsync(user, "Estudiante");

        // 5. RETORNAR DTO con datos básicos del usuario registrado
        return new UserRegistrationResponseDto(
            user.Id,
            user.Email!,
            nombreCompleto);
    }

    // ─── LOGIN (Autenticación) ────────────────────────────────────────────

    /// <summary>
    /// Autentica un usuario validando sus credenciales y generando tokens.
    /// 1. Busca el usuario por email
    /// 2. Valida la contraseña
    /// 3. Obtiene sus roles
    /// 4. Genera JWT + Refresh Token
    /// 5. Retorna todo empaquetado en AuthResponseDto
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        // 1. BUSCAR usuario por email
        var user = await userManager.FindByEmailAsync(dto.Email);

        // 2. VALIDAR credenciales
        // Mismo mensaje para ambos errores → evita enumeración de usuarios
        if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
            throw new AppException(
                "Credenciales incorrectas",
                HttpStatusCode.BadRequest);

        // 3. OBTENER roles del usuario
        var roles = await userManager.GetRolesAsync(user);

        // 4. GENERAR tokens
        var accessToken = GenerateJwtToken(user, roles);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        var minutes = int.Parse(configuration["Jwt:AccessTokenMinutes"] ?? "60");

        // 5. EMPAQUETAR y retornar
        return new AuthResponseDto(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(minutes),
            new UserDto(user.Id, user.Email!, user.NombreCompleto, roles));
    }

    // ─── REFRESH (Token Rotation) ──────────────────────────────────────────

    /// <summary>
    /// Renueva los tokens usando un refresh token válido.
    /// Implementa token rotation para mayor seguridad:
    /// 1. Valida el refresh token
    /// 2. Lo revoca (no se puede reutilizar)
    /// 3. Genera nuevos JWT y refresh token
    /// 4. Retorna todo empaquetado
    /// </summary>
    public async Task<RefreshTokenResponseDto> RefreshAsync(
        RefreshTokenRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        // 1. VALIDAR refresh token
        var tokenEntity = await ValidateRefreshTokenAsync(dto.RefreshToken);

        if (tokenEntity is null)
            throw new AppException(
                "Refresh token inválido o expirado",
                HttpStatusCode.BadRequest);

        var user = tokenEntity.User!;

        // 2. REVOCAR el token usado (token rotation - mayor seguridad)
        await RevokeRefreshTokenAsync(dto.RefreshToken);

        // 3. GENERAR nuevos tokens
        var roles = await userManager.GetRolesAsync(user);
        var newAccessToken = GenerateJwtToken(user, roles);
        var newRefreshToken = await GenerateRefreshTokenAsync(user);

        var minutes = int.Parse(configuration["Jwt:AccessTokenMinutes"] ?? "60");

        // 4. EMPAQUETAR y retornar
        return new RefreshTokenResponseDto(
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(minutes));
    }

    // ─── Generación de JWT ────────────────────────────────────────────────

    public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSection = configuration.GetSection("Jwt");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSection["Key"]!));

        // C# 13: collection expression para claims base
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new("nombreCompleto",              user.NombreCompleto),
        ];

        // Un Claim por cada rol del usuario
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var minutes = int.Parse(jwtSection["AccessTokenMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: new SigningCredentials(
                                    key,
                                    SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ─── Refresh tokens ───────────────────────────────────────────────────

    public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        var days = int.Parse(
            configuration["Jwt:RefreshTokenDays"] ?? "7");

        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(days),
            IsRevoked = false
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken.Token;
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

        // Property pattern — C# 13: verifica IsActive en una sola expresión
        if (refreshToken is not { IsActive: true })
            return null;

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken is null) return;

        refreshToken.IsRevoked = true;
        await context.SaveChangesAsync();
    }
}