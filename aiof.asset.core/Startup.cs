using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

using aiof.asset.data;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<AiofContext>(o => o.UseNpgsql(_config[Keys.DataPostgreSQL], o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            services.AddHealthChecks();
            services.AddFeatureManagement();
            services.AddLogging()
                .AddHttpContextAccessor()
                .AddApplicationInsightsTelemetry()
                .AddAiofAuthentication()
                .AddAiofSwaggerGen();

            services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.WriteIndented = true;
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseCors(x => x.WithOrigins(_config[Keys.CorsPortal]).AllowAnyHeader().AllowAnyMethod());
            }

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
