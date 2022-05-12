using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Minibank.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        public readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(new {Message = "Внутренняя ошибка сервера!"});
            }
        }
    }
}