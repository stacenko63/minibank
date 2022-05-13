using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Minibank.Web.Middlewares
{

    public class JwtTokenContent
    {
        public int? exp { get; set; }
    }

    public class CustomAuthenticationMiddleware
    {
        public readonly RequestDelegate next;

        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            var token = httpContext.Request.Headers[HeaderNames.Authorization];

            if (!StringValues.IsNullOrEmpty(token))
            {
                
                var tokenResult = token.ToString().Substring(7);

                var payload = tokenResult.Split('.')[1];

                payload = payload.Replace('-', '+'); 
                payload = payload.Replace('_', '/'); 
                
                switch (payload.Length % 4) 
                {
                    case 0: break; 
                    case 2: payload += "=="; break; 
                    case 3: payload += "="; break; 
                    default: throw new Exception();
                }
                
                var converted = Convert.FromBase64String(payload);
                
                var jsonPayload = Encoding.UTF8.GetString(converted);
                
                var r = JsonSerializer.Deserialize<JwtTokenContent>(jsonPayload);
                
                var exp = r.exp;

                if (exp != null)
                {

                    DateTime dateByExp = new DateTime(1970, 1, 1).AddSeconds((double) exp);

                    if (DateTime.UtcNow > dateByExp)
                    {
                        httpContext.Response.StatusCode = 403;
                        await httpContext.Response.WriteAsJsonAsync(new {Message = "Expired token!"});
                        return;
                    }

                }

            }

            await next(httpContext);

        }
    }
}

