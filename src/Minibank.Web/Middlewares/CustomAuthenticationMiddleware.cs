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

            var token = await httpContext.GetTokenAsync("access_token");

            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();

                var jwtToken = handler.ReadJwtToken(token);

                var exp = jwtToken.Payload.Exp;

                DateTime dateByExp = new DateTime(1970, 1, 1).AddSeconds((double) exp);

                if (DateTime.UtcNow > dateByExp)
                {
                    httpContext.Response.StatusCode = 403;
                    await httpContext.Response.WriteAsJsonAsync(new {Message = "Expired token!"});
                    return;
                }

            }

            await next(httpContext);

        }
    }
}

