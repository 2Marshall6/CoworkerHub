using CoworkerHub.API.Models;
using CoworkerHub.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace CoworkerHub.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                AppValidationException => 400,
                NotFoundException => 404,
                UnauthorizedException => 401,
                AlreadyExistsException => 409,
                _ => (int)HttpStatusCode.InternalServerError,
            };
            var errorResponse = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }

    }
}