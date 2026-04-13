using Api.Attributes;
using Application.Common.Models;
using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AuthController(
    IAuthService authService) : BaseApiController
{
    // ── POST /api/auth/register ────────────────────────────────────────

    [HttpPost("register")]
    [AllowAnonymous]
    [Validate<RegisterDto>]
    [ProducesResponseType(typeof(ApiResponse<UserRegistrationResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<UserRegistrationResponseDto>>> Register(
        [FromBody] RegisterDto dto,
        CancellationToken cancellationToken)
    {
        // ✅ TODA LA LÓGICA ESTÁ EN authService
        var response = await authService.RegisterAsync(dto, cancellationToken);

        return Created(response, nameof(Login), new { }, "Usuario registrado correctamente");
    }

    // ── POST /api/auth/login ───────────────────────────────────────────

    [HttpPost("login")]
    [AllowAnonymous]
    [Validate<LoginDto>]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        // ✅ TODA LA LÓGICA ESTÁ EN authService
        var response = await authService.LoginAsync(dto, cancellationToken);

        return Success(response, "Login exitoso");
    }

    // ── POST /api/auth/refresh ─────────────────────────────────────────

    [HttpPost("refresh")]
    [AllowAnonymous]
    [Validate<RefreshTokenRequestDto>]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> Refresh(
        [FromBody] RefreshTokenRequestDto dto,
        CancellationToken cancellationToken)
    {
        // ✅ TODA LA LÓGICA ESTÁ EN authService
        var response = await authService.RefreshAsync(dto, cancellationToken);

        return Success(response, "Tokens renovados");
    }

    // ── POST /api/auth/logout ──────────────────────────────────────────

    [HttpPost("logout")]
    [Authorize]
    [Validate<RefreshTokenRequestDto>]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<bool>>> Logout(
        [FromBody] RefreshTokenRequestDto dto,
        CancellationToken cancellationToken)
    {
        await authService.RevokeRefreshTokenAsync(dto.RefreshToken);
        return Success(true, "Sesión cerrada correctamente");
    }
}