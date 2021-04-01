using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

using aiof.asset.data;

namespace aiof.asset.core
{
    public static partial class MiddlewareExtensions
    {
        public static IServiceCollection AddAiofAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                Keys.Bearer,
                x =>
                {
                    var rsa = RSA.Create();
                    rsa.FromXmlString(Startup._config[Keys.JwtPublicKey]);

                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Startup._config[Keys.JwtIssuer],
                        ValidateAudience = true,
                        ValidAudience = Startup._config[Keys.JwtAudience],
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
                });

            return services;
        }

        public static IServiceCollection AddAiofSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(Startup._config[Keys.OpenApiVersion], new OpenApiInfo
                {
                    Title = Startup._config[Keys.OpenApiTitle],
                    Version = Startup._config[Keys.OpenApiVersion],
                    Description = Startup._config[Keys.OpenApiDescription],
                    Contact = new OpenApiContact
                    {
                        Name = Startup._config[Keys.OpenApiContactName],
                        Email = Startup._config[Keys.OpenApiContactEmail],
                        Url = new Uri(Startup._config[Keys.OpenApiContactUrl])
                    },
                    License = new OpenApiLicense
                    {
                        Name = Startup._config[Keys.OpenApiLicenseName],
                        Url = new Uri(Startup._config[Keys.OpenApiLicenseUrl]),
                    }
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            return services;
        }
    }
}
