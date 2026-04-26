using FluentValidation;
using System.Text.Json;

namespace Financas.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Tenta processar a requisição normalmente
            await _next(context);
        }
        catch (ValidationException ex)
        {
            // Captura APENAS os erros do FluentValidation (Regras de Negócio)
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex) 
        {
            // Captura erros de login/acesso negado
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            // Captura qualquer outro erro inesperado (Bugs)
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new
        {
            Titulo = "Um ou mais erros de validação ocorreram.",
            Status = context.Response.StatusCode,
            Erros = errors
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        var response = new
        {
            Titulo = "Acesso Negado",
            Status = context.Response.StatusCode,
            Detalhe = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            Titulo = "Ocorreu um erro interno no servidor.",
            Status = context.Response.StatusCode,
            Detalhe = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}