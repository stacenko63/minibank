using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Minibank.Web.Middlewares
{
    public class CustomAuthenticationMiddleware
    {
        public readonly RequestDelegate next;

        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
                if (httpContext.Response.StatusCode == 401) throw new Exception("You are not Authenticate!");
                
                var token = await httpContext.GetTokenAsync("access_token");
                var handler = new JwtSecurityTokenHandler();

                if (DateTime.UtcNow > handler.ReadToken(token).ValidTo)
                {
                    httpContext.Response.StatusCode = 403;
                    throw new Exception("Expired token!");
                }
            }
            catch (Exception exception)
            {
                await httpContext.Response.WriteAsJsonAsync(new {Message = exception.Message});
            }
        }
    }
}