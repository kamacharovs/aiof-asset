using System;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.core
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public static IConfiguration _config;
        public static IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAssetRepository, AssetRepository>()
                .AddScoped<IAssetStockRepository, AssetStockRepository>()
                .AddScoped<IAssetHomeRepository, AssetHomeRepository>()
                .AddScoped<IEventRepository, EventRepository>()
                .AddScoped<IEnvConfiguration, EnvConfiguration>()
                .AddScoped<ITenant, Tenant>()
                .AddAutoMapper(typeof(AutoMappingProfile).Assembly);

            services.AddDbContext<AssetContext>(o => o.UseNpgsql(_config[Keys.DataPostgreSQL], o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            services.AddHealthChecks();
            services.AddFeatureManagement();
            services.AddLogging()
                .AddHttpContextAccessor()
                .AddApplicationInsightsTelemetry()
                .AddAssetAuthentication()
                .AddAssetSwaggerGen()
                .AddAssetFluentValidators()
                .AddAssetApiVersioning()
                .AddEventingRestClient();

            services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.WriteIndented = true;
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseCors(x => x.WithOrigins(_config[Keys.CorsPortal]).AllowAnyHeader().AllowAnyMethod());
            }

            app.UseAssetExceptionMiddleware();
            app.UseAssetUnauthorizedMiddleware();
            app.UseHealthChecks("/health");
            app.UseSwagger();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
