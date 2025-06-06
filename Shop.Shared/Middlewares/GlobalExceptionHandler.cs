using Shop.Shared.Models;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Shop.Shared.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            context.Response.ContentType = "application/json";

            ErrorResponse response = new();

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid data supplied";
                response.Errors = validationException.Errors.Select(e => new FieldError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                })
                .ToArray();
            }
            else if (exception is AppException appException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = exception.Message;
            }
            else
            {
                logger.LogError(exception.Message, exception);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An internal server error happened";
            }
            await context.Response.WriteAsJsonAsync(response);

            return true;
        }
    }
}
