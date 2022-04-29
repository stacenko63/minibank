using System;
using System.Linq;
using System.Reflection.Metadata;
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
                await httpContext.Response.WriteAsJsonAsync(exception.Errors.Select(x => x.ErrorMessage).ToList());
            }
            catch (NullReferenceException)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsJsonAsync("You shouldn't use null values!");
            }
        }

    }
}