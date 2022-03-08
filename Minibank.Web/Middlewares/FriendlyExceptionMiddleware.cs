using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Minibank.Core;

namespace Minibank.Web.Middlewares
{
    public class FriendlyExceptionMiddleware
    {
        public readonly RequestDelegate next;

        public FriendlyExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (UserFriendlyException exception)
            {
                await httpContext.Response.WriteAsync(exception.What());
            }
        }

    }
}