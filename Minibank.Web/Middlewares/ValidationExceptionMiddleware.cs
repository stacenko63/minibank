using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Minibank.Core;

namespace Minibank.Web.Middlewares
{
    public class ValidationExceptionMiddleware
    {
        public readonly RequestDelegate next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (ValidationException exception)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsJsonAsync(exception.Message);
            }
            catch (FluentValidation.ValidationException exception)
            {
                httpContext.Response.StatusCode = 400;
                var errors = exception.Errors.Select(x => x.ErrorMessage);
                await httpContext.Response.WriteAsJsonAsync(new {Error = string.Join(Environment.NewLine, errors)});
            }
        }

    }
}