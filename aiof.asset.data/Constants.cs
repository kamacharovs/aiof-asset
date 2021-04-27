using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace aiof.asset.data
{
    public static class Constants
    {
        public const string Accept = nameof(Accept);
        public const string ApplicationJson = "application/json";
        public const string ApplicationProblemJson = "application/problem+json";

        public const string DefaultMessage = "An unexpected error has occurred";
        public const string DefaultValidationMessage = "One or more validation errors have occurred. Please see errors for details";
        public const string DefaultUnauthorizedMessage = "Unauthorized. Missing, invalid or expired credentials provided";
        public const string DefaultForbiddenMessage = "Forbidden. You don't have enough permissions to access this API";

        public static int[] AllowedUnauthorizedStatusCodes = new int[]
        {
            StatusCodes.Status401Unauthorized,
            StatusCodes.Status403Forbidden
        };

        public static JsonSerializerOptions JsonSerializerSettings
            => new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            };
    }

    public static class Keys
    {
        public const string Data = nameof(Data);
        public const string PostgreSQL = nameof(PostgreSQL);
        public const string DataPostgreSQL = Data + ":" + PostgreSQL;

        public const string Cors = nameof(Cors);
        public const string Portal = nameof(Portal);
        public const string CorsPortal = Cors + ":" + Portal;

        public const string Jwt = nameof(Jwt);
        public const string Bearer = nameof(Bearer);
        public const string Issuer = nameof(Issuer);
        public const string Audience = nameof(Audience);
        public const string PublicKey = nameof(PublicKey);
        public const string JwtIssuer = nameof(Jwt) + ":" + nameof(Issuer);
        public const string JwtAudience = nameof(Jwt) + ":" + nameof(Audience);
        public const string JwtPublicKey = nameof(Jwt) + ":" + nameof(PublicKey);

        public const string OpenApi = nameof(OpenApi);
        public const string Version = nameof(Version);
        public const string Title = nameof(Title);
        public const string Description = nameof(Description);
        public const string Contact = nameof(Contact);
        public const string Name = nameof(Name);
        public const string Email = nameof(Email);
        public const string Url = nameof(Url);
        public const string License = nameof(License);
        public const string OpenApiVersion = nameof(OpenApi) + ":" + nameof(Version);
        public const string OpenApiTitle = nameof(OpenApi) + ":" + nameof(Title);
        public const string OpenApiDescription = nameof(Description) + ":" + nameof(Description);
        public const string OpenApiContactName = nameof(OpenApi) + ":" + nameof(Contact) + ":" + nameof(Name);
        public const string OpenApiContactEmail = nameof(OpenApi) + ":" + nameof(Contact) + ":" + nameof(Email);
        public const string OpenApiContactUrl = nameof(OpenApi) + ":" + nameof(Contact) + ":" + nameof(Url);
        public const string OpenApiLicenseName = nameof(OpenApi) + ":" + nameof(License) + ":" + nameof(Name);
        public const string OpenApiLicenseUrl = nameof(OpenApi) + ":" + nameof(License) + ":" + nameof(Url);

        public static class Claim
        {
            public const string UserId = "user_id";
            public const string ClientId = "client_id";
            public const string PublicKey = "public_key";
        }

        public static class Entity
        {
            public static string Asset = nameof(data.Asset).ToSnakeCase();
            public static string AssetStock = nameof(data.AssetStock).ToSnakeCase();
            public static string AssetType = nameof(data.AssetType).ToSnakeCase();
            public static string AssetSnapshot = nameof(data.AssetSnapshot).ToSnakeCase();
            public static string AssetStockSnapshot = nameof(data.AssetStockSnapshot).ToSnakeCase();
        }
    }

    public static class AssetTypes
    {
        public const string Car = "car";
        public const string House = "house";
        public const string Investment = "investment";
        public const string Stock = "stock";
        public const string Cash = "cash";
        public const string Other = "other";
    }
}
