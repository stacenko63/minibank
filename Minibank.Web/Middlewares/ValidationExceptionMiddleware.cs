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
                
            }
            catch (FluentValidation.ValidationException exception)
            {
                httpContext.Response.StatusCode = 400;
                var errors = exception.Errors.Select(x => x.ErrorMessage).ToList();
                await httpContext.Response.WriteAsJsonAsync(errors[0]);
            }
            catch (NullReferenceException exception)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsJsonAsync("You shouldn't use null values!");
            }
        }

    }
}