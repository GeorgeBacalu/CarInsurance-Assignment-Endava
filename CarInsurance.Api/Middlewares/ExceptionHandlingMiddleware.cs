using CarInsurance.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;

namespace CarInsurance.Api.Middlewares;
public class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            ProblemDetails problem = exception switch
            {
                KeyNotFoundException ex => new() { Title = "Key not found", Detail = ex.Message, Status = StatusCodes.Status404NotFound },
                BadRequestException ex => new() { Title = "Bad request", Detail = ex.Message, Status = StatusCodes.Status400BadRequest },
                _ => new() { Title = "Internal server error", Detail = exception.Message, Status = StatusCodes.Status500InternalServerError }
            };
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = problem?.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
