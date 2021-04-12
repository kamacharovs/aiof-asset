using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using aiof.asset.data;

namespace aiof.asset.core
{
    public class AssetUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public AssetUnauthorizedMiddleware(
            RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);
            await WriteUnauthorizedResponseAsync(httpContext);
        }

        public async Task WriteUnauthorizedResponseAsync(
            HttpContext httpContext)
        {
            if (Constants.AllowedUnauthorizedStatusCodes.Contains(httpContext.Response.StatusCode) is false)
                return;

            var statusCode = httpContext.Response.StatusCode;
            var problem = new AssetProblemDetailBase();

            switch (statusCode)
            {
                case StatusCodes.Status401Unauthorized:
                    problem.Code = StatusCodes.Status401Unauthorized;
                    problem.Message = Constants.DefaultUnauthorizedMessage;
                    break;
                case StatusCodes.Status403Forbidden:
                    problem.Code = StatusCodes.Status403Forbidden;
                    problem.Message = Constants.DefaultForbiddenMessage;
                    break;
            }

            var problemJson = JsonSerializer
                .Serialize(problem, Constants.JsonSerializerSettings);

            httpContext.Response.ContentType = Constants.ApplicationProblemJson;

            await httpContext.Response
                .WriteAsync(problemJson);
        }
    }
}