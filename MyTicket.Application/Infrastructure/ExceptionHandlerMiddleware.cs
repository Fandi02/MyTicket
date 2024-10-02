using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Models;
using ValidationException = MyTicket.Application.Exceptions.ValidationException;

namespace MyTicket.Application.Infrastructure;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var executingEndpoint = context.GetEndpoint();

        bool hasIgnoreManipulateResult = false;
        if (executingEndpoint != null)
        {
            var att = executingEndpoint.Metadata.OfType<IgnoreResultManipulatorAttribute>();
            if (att.Any())
                hasIgnoreManipulateResult = true;
        }

        ResultApi result = new ResultApi
        {
            IsSuccess = false,
            Path = context.Request.Path.HasValue ? context.Request.Path.Value : "",
            Method = context.Request.Method
        };

        bool errorBecauseOfValidationException = false;
        var logError = false;
        switch (ex)
        {
            case ValidationException validationException:
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                result.Payload = validationException.Failures;
                result.Message = validationException.Message;
                errorBecauseOfValidationException = true;
                break;
            case BadRequestException badRequestException:
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                result.Message = badRequestException.Message;
                break;
            case NotFoundException notFoundException:
                result.StatusCode = (int)HttpStatusCode.NotFound;
                result.Message = notFoundException.Message;
                break;
            case UnauthorizeException unauthorizeException:
                result.StatusCode = (int)HttpStatusCode.Unauthorized;
                result.Message = unauthorizeException.Message;
                break;
            case ForbiddenException forbiddenException:
                result.StatusCode = (int)HttpStatusCode.Forbidden;
                result.Message = forbiddenException.Message;
                break;
            default:
                logError = true;
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;

                if (ex.InnerException != null)
                {
                    result.InnerMessage = ex.InnerException.Message;
                }
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = result.StatusCode;

        if (logError && (result.StatusCode < 200 || result.StatusCode >= 300))
        {
            var loggerOptions = context.RequestServices.GetRequiredService<ExceptionHandlerMiddlewareOptions>();
            if (loggerOptions.EnableLogErrorExceptionHandler)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionHandlerMiddleware>>();
                logger.LogError(ex, "Exception {msg}", result.Message);
            }
        }

        var jsonSetting = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (errorBecauseOfValidationException)
            jsonSetting.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;

        if (!hasIgnoreManipulateResult)
            return context.Response.WriteAsync(JsonSerializer.Serialize(result, jsonSetting));
        else
            return context.Response.WriteAsync(JsonSerializer.Serialize(result.Message));
    }
}

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}

public class ExceptionHandlerMiddlewareOptions
{
    public ExceptionHandlerMiddlewareOptions(IConfiguration configuration)
    {
        EnableLogErrorExceptionHandler = configuration.GetValue<bool?>("EnableLogErrorExceptionHandler") ?? false;
    }
    public bool EnableLogErrorExceptionHandler { get; set; }
}