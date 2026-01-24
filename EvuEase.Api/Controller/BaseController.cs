using EvuEase.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Success<T>(T data, int statusCode = 200)
    {
        var response = ApiResponse<T>.SuccessResponse(data);
        return StatusCode(statusCode, response);
    }

    protected IActionResult Success(int statusCode = 200)
    {
        var response = ApiResponse<object?>.SuccessResponse(null);
        return StatusCode(statusCode, response);
    }

    protected IActionResult Error(string message, int statusCode = 400, object? details = null)
    {
        var response = ApiResponse<object>.ErrorResponse(message, statusCode, details);
        return StatusCode(statusCode, response);
    }

    protected IActionResult BadRequest(string message, object? details = null)
    {
        return Error(message, 400, details);
    }

    protected IActionResult Unauthorized(string message = "Unauthorized", object? details = null)
    {
        return Error(message, 401, details);
    }

    protected IActionResult Forbidden(string message = "Forbidden", object? details = null)
    {
        return Error(message, 403, details);
    }

    protected IActionResult NotFound(string message = "Resource not found", object? details = null)
    {
        return Error(message, 404, details);
    }

    protected IActionResult Conflict(string message, object? details = null)
    {
        return Error(message, 409, details);
    }

    protected IActionResult InternalServerError(string message = "An error occurred while processing your request", object? details = null)
    {
        return Error(message, 500, details);
    }

    protected IActionResult Created<T>(T data, string? location = null)
    {
        var response = ApiResponse<T>.SuccessResponse(data);
        if (!string.IsNullOrEmpty(location))
        {
            return Created(location, response);
        }
        return StatusCode(201, response);
    }

    protected new IActionResult NoContent()
    {
        return base.NoContent();
    }

    protected IActionResult Ok<T>(T data)
    {
        return Success(data, 200);
    }
}

