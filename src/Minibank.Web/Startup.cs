using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Minibank.Core;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Minibank.Data;
using Minibank.Web.Middlewares;

namespace Minibank.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Minibank.Web", Version = "v1"});
                
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("https://demo.duendesoftware.com/connect/token"),
                            Scopes = new Dictionary<string, string>()
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = SecuritySchemeType.OAuth2.GetDisplayName(),
                            }
                        },
                        new List<string>()
                    }
                });
            });
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Audience = "api";
                options.Authority = "https://demo.duendesoftware.com";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateAudience = false
                };
            });
            
            services.AddControllers();
            
            services.AddScoped<IDatabase, Database>();
            services.AddData(Configuration);
            services.AddCore();

            
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ValidationExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minibank.Web v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseMiddleware<CustomAuthenticationMiddleware>();
            
            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}