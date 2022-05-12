using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Minibank.Core;

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

                var token = await httpContext.GetTokenAsync("access_token");
                
                if (token != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                
                    if (DateTime.UtcNow > handler.ReadToken(token).ValidTo)
                    {
                        httpContext.Response.StatusCode = 403;
                        throw new CustomAuthenticationException("Expired token!");
                    }

                }
                
                await next(httpContext);
                
            }
            catch (CustomAuthenticationException exception)
            {
                await httpContext.Response.WriteAsJsonAsync(new {Message = exception.Message});
            }
        }
    }
}