using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

using FluentValidation;

using aiof.asset.data;

namespace aiof.asset.core
{
    public static partial class MiddlewareExtensions
    {
        public static IServiceCollection AddAssetAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                Keys.Bearer,
                x =>
                {
                    var rsa = RSA.Create();
                    rsa.FromXmlString(Startup._config[Keys.JwtPublicKey]);

                    x.TokenValidationParameters = new TokenValidationParameters
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

        public static IServiceCollection AddAssetSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(Constants.ApiV1, new OpenApiInfo
                {
                    Title = Startup._config[Keys.OpenApiTitle],
                    Version = Constants.ApiV1,
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

        public static IServiceCollection AddAssetFluentValidators(this IServiceCollection services)
        {
            services.AddScoped<AbstractValidator<string>, AssetTypeValidator>()
                .AddScoped<AbstractValidator<AssetDto>, AssetDtoValidator>()
                .AddScoped<AbstractValidator<AssetSnapshotDto>, AssetSnapshotDtoValidator>()
                .AddScoped<AbstractValidator<AssetStockDto>, AssetStockDtoValidator>();

            return services;
        }

        public static IServiceCollection AddAssetApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = ApiVersion.Parse(Constants.ApiV1);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
                x.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            return services;
        }

        public static IApplicationBuilder UseAssetExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AssetExceptionMiddleware>();
        }

        public static IApplicationBuilder UseAssetUnauthorizedMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AssetUnauthorizedMiddleware>();
        }
    }
}
