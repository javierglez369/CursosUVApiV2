using Application.Common.Models;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    // ── Helpers de respuesta ──────────────────────────────────────────────────
    protected ActionResult<ApiResponse<T>> Success<T>(
        T data,
        string? message = null,
        int statusCode = 200)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message);
        response.StatusCode = statusCode;
        return StatusCode(statusCode, response);
    }

    protected ActionResult<ApiResponse<T>> Created<T>(
        T data,
        string actionName,
        object routeValues,
        string? message = null)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message);
        response.StatusCode = 201;
        return CreatedAtAction(actionName, routeValues, response);
    }

    protected ActionResult<ApiResponse<object>> SuccessAnonymous(
        object data,
        string? message = null)
    {
        var response = ApiResponse<object>.SuccessResponse(data, message);
        return Ok(response);
    }

    // ── Helpers de excepción ──────────────────────────────────────────────────

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    protected void ThrowNotFound(string message)
        => throw AppException.NotFound(message);

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    protected void ThrowBadRequest(string message, List<string>? errors = null)
    {
        if (errors is not null && errors.Count > 0)
            throw AppException.BadRequest(message, errors);
        throw AppException.BadRequest(message);
    }

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    protected void ThrowForbidden(string message)
        => throw AppException.Forbidden(message);

    protected void ValidateModelState()
    {
        if (ModelState.IsValid) return;

        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        throw new ValidationException(errors);
    }
}