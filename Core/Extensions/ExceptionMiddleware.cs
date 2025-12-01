using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Core.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (System.Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, System.Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = "Internal Server Error";

            if (e is ValidationException validationException)
            {
                message = validationException.Message;
                IEnumerable<ValidationFailure> errors = validationException.Errors;
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var validationErrorDetails = new ValidationErrorDetails
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = message,
                    Errors = errors
                };

                return httpContext.Response.WriteAsync(validationErrorDetails.ToString());
            }

            var errorDetails = new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = message
            };

            return httpContext.Response.WriteAsync(errorDetails.ToString());
        }
    }
}
