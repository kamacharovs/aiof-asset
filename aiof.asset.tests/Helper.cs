using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

using AutoMapper;
using FluentValidation;
using Moq;
using Bogus;
using RestSharp;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.tests
{
    [ExcludeFromCodeCoverage]
    public class ServiceHelper
    {
        public int? UserId { get; set; }
        public int? ClientId { get; set; }
        public Guid? PublicKey { get; set; }

        public Dictionary<string, string> ConfigurationDict
            => new Dictionary<string, string>
            {
                { Keys.EventingBaseUrl, "http://test" },
                { Keys.EventingFunctionKeyHeaderName, "x-functions-key" },
                { Keys.EventingFunctionKey, "functionkey" },
                { "FeatureManagement:Eventing", "true" }
            };

        public T GetRequiredService<T>()
        {
            var provider = Services().BuildServiceProvider();

            provider.GetRequiredService<FakeDataManager>()
                .UseFakeContext();

            return provider.GetRequiredService<T>();
        }

        public ServiceCollection Services()
        {
            var services = new ServiceCollection();

            services.AddScoped<IAssetRepository, AssetRepository>()
                .AddScoped<IAssetStockRepository, AssetStockRepository>()
                .AddScoped<IAssetHomeRepository, AssetHomeRepository>()
                .AddScoped<IEventRepository, EventRepository>()
                .AddScoped<IEnvConfiguration, EnvConfiguration>()
                .AddScoped<ITenant, Tenant>()
                .AddScoped<FakeDataManager>();

            services.AddScoped(x => GetMockTenant());
            services.AddSingleton(x => GetMockRestClient<object>());
            services.AddSingleton(new MapperConfiguration(x =>
                {
                    x.AddProfile(new AutoMappingProfile());
                    x.AddProfile(new AssetEventProfile());
                })
                .CreateMapper());

            services.AddScoped<IConfiguration>(x =>
            {
                var configurationBuilder = new ConfigurationBuilder();

                configurationBuilder.AddInMemoryCollection(ConfigurationDict);

                return configurationBuilder.Build();
            });

            services.AddDbContext<AssetContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddScoped<AbstractValidator<string>, AssetTypeValidator>()
                .AddScoped<AbstractValidator<AssetDto>, AssetDtoValidator>()
                .AddScoped<AbstractValidator<AssetSnapshotDto>, AssetSnapshotDtoValidator>()
                .AddScoped<AbstractValidator<AssetStockDto>, AssetStockDtoValidator>()
                .AddScoped<AbstractValidator<AssetHomeDto>, AssetHomeDtoValidator>();

            services.AddLogging();
            services.AddFeatureManagement();
            services.AddHttpContextAccessor();

            return services;
        }

        public ITenant GetMockTenant()
        {
            var userId = UserId ?? 1;
            var clientId = ClientId ?? 1;
            var publicKey = PublicKey ?? Guid.NewGuid();

            var ci = new ClaimsIdentity();
            ci.AddClaim(new Claim(Keys.Claim.UserId, userId.ToString()));
            ci.AddClaim(new Claim(Keys.Claim.ClientId, clientId.ToString()));
            ci.AddClaim(new Claim(Keys.Claim.PublicKey, publicKey.ToString()));
            var user = new ClaimsPrincipal(ci);

            var mockedHttpContext = new Mock<HttpContext>();
            mockedHttpContext.Setup(x => x.User).Returns(user);

            return new Tenant(mockedHttpContext.Object);
        }

        public IRestClient GetMockRestClient<T>()
            where T : new()
        {
            var mockedRestClient = new Mock<IRestClient>();
            var mockedRestResponse = new Mock<IRestResponse<T>>();

            mockedRestClient.Setup(x => x.ExecuteAsync<T>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockedRestResponse.Object);

            return mockedRestClient.Object;
        }
    }

    [ExcludeFromCodeCoverage]
    public static class Helper
    {
        public const string Category = nameof(Category);
        public const string UnitTest = nameof(UnitTest);
        public const string IntegrationTest = nameof(IntegrationTest);

        #region Unit Tests
        public static FakeDataManager _Fake
            => new ServiceHelper().GetRequiredService<FakeDataManager>() ?? throw new ArgumentNullException(nameof(FakeDataManager));

        public static int GeneratedAmount = 3;

        public static IEnumerable<object[]> AssetsIdUserId()
        {
            return _Fake.GetFakeAssetsData(
                id: true,
                userId: true);
        }
        public static IEnumerable<object[]> AssetsStockIdUserId()
        {
            return _Fake.GetFakeAssetsStockData(
                id: true,
                userId: true);
        }

        public static IEnumerable<object[]> AssetsHomeIdUserId()
        {
            return _Fake.GetFakeAssetsHomeData(
                id: true,
                userId: true);
        }

        public static IEnumerable<object[]> AssetsId()
        {
            return _Fake.GetFakeAssetsData(
                id: true);
        }

        public static IEnumerable<object[]> AssetsUserId()
        {
            return _Fake.GetFakeAssetsData(
                userId: true);
        }

        public static IEnumerable<object[]> ValidNames()
        {
            var validNames = new List<object[]>();

            validNames.Add(new object[] { "valid name" });
            validNames.Add(new object[] { "Another Valid Name" });

            return validNames;
        }

        public static IEnumerable<object[]> InvalidNames()
        {
            var invalidNames = new List<object[]>();

            invalidNames.Add(new object[] { "" });
            invalidNames.Add(new object[] { Repeat("nametoolong") });

            return invalidNames;
        }

        public static IEnumerable<object[]> ValidTypeNames()
        {
            var validTypeNames = new List<object[]>();

            var validAssetTypeNames = _Fake.GetFakeAssetTypes()
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            foreach (var assetTypeName in validAssetTypeNames)
                validTypeNames.Add(new object[] { assetTypeName });

            return validTypeNames;
        }
        public static IEnumerable<object[]> InvalidTypeNames()
        {
            return new List<object[]>
            {
                new object[] { "" },
                new object[] { "test" },
                new object[] { Repeat("typenametoolong") }
            };
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return new List<object[]>
            {
                new object[] { 1 },
                new object[] { 50 },
                new object[] { CommonValidator.MaximumValue - 1 },
                new object[] { CommonValidator.MaximumValue - 50 }
            };
        }
        public static IEnumerable<object[]> InvalidValues()
        {
            return new List<object[]>
            {
                new object[] { -1 },
                new object[] { -50 },
                new object[] { CommonValidator.MaximumValue + 1 },
                new object[] { CommonValidator.MaximumValue + 50 }
            };
        }

        public static IEnumerable<object[]> ValidTickerSymbols()
        {
            return new List<object[]>
            {
                new object[] { "VTSAX" },
                new object[] { "TSLA" },
                new object[] { "MAYBEASYMBOL" },
                new object[] { "apple" }
            };
        }
        public static IEnumerable<object[]> InvalidTickerSymbols()
        {
            return new List<object[]>
            {
                new object[] { "" },
                new object[] { Repeat("VTSAX", 50) }
            };
        }

        public static IEnumerable<object[]> ValidShares()
        {
            return new List<object[]>()
            {
                new object[] { 0.0875 },
                new object[] { 1 },
                new object[] { 105.69 }
             };
        }
        public static IEnumerable<object[]> InvalidShares()
        {
            return new List<object[]>
            {
                new object[] { null },
                new object[] { 0 },
                new object[] { -1 },
                new object[] { -230.234 }
            };
        }

        public static IEnumerable<object[]> ValidExpenseRatios()
        {
            return new List<object[]>
            {
                new object[] { 0.025 },
                new object[] { 0.0015 }
            };
        }
        public static IEnumerable<object[]> InvalidExpenseRatios()
        {
            return new List<object[]>
            {
                new object[] { 0 },
                new object[] { -0.025 },
                new object[] { -101 },
                new object[] { 101 },
                new object[] { 123456 }
            };
        }

        public static IEnumerable<object[]> ValidDividendYields()
        {
            return new List<object[]>
            {
                new object[] { 0.025 },
                new object[] { 0.0015 }
            };
        }
        public static IEnumerable<object[]> InvalidDividendYields()
        {
            return new List<object[]>
            {
                new object[] { 0 },
                new object[] { -0.025 },
                new object[] { -101 },
                new object[] { 101 },
                new object[] { 123456 }
            };
        }

        public static string Repeat(string s, int n = 101)
        {
            return string.Concat(Enumerable.Repeat(s, n));
        }

        public static AssetDto RandomAssetDto()
        {
            return FakerAssetDto().Generate();
        }
        public static List<AssetDto> RandomAssetDtos(int? n = null)
        {
            return FakerAssetDto().Generate(n ?? GeneratedAmount);
        }
        private static Faker<AssetDto> FakerAssetDto()
        {
            var validAssetTypes = _Fake.GetFakeAssetTypes()
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            return new Faker<AssetDto>()
                .RuleFor(x => x.Name, f => f.Random.String2(10))
                .RuleFor(x => x.TypeName, f => f.Random.ListItem(validAssetTypes))
                .RuleFor(x => x.Value, f => f.Random.Int(1000, 10000));
        }

        public static AssetStockDto RandomAssetStockDto()
        {
            return FakerAssetStockDto().Generate();
        }
        public static List<AssetStockDto> RandomAssetStockDtos(int? n = null)
        {
            return FakerAssetStockDto().Generate(n ?? GeneratedAmount);
        }
        private static Faker<AssetStockDto> FakerAssetStockDto()
        {
            return new Faker<AssetStockDto>()
                .RuleFor(x => x.Name, f => f.Random.String2(10))
                .RuleFor(x => x.TypeName, f => AssetTypes.Stock)
                .RuleFor(x => x.Value, f => f.Random.Int(1000, 10000))
                .RuleFor(x => x.TickerSymbol, f => f.Random.String2(5))
                .RuleFor(x => x.Shares, f => Math.Round(f.Random.Decimal(10, 150), 2))
                .RuleFor(x => x.ExpenseRatio, f => Math.Round(f.Random.Decimal(0.001M, 0.005M), 5))
                .RuleFor(x => x.DividendYield, f => Math.Round(f.Random.Decimal(0.001M, 0.05M), 5));
        }

        public static AssetHomeDto RandomAssetHomeDto()
        {
            return FakerAssetHomeDtos().Generate();
        }
        public static List<AssetHomeDto> RandomAssetHomeDtos(int? n = null)
        {
            return FakerAssetHomeDtos().Generate(n ?? GeneratedAmount);
        }
        private static Faker<AssetHomeDto> FakerAssetHomeDtos()
        {
            return new Faker<AssetHomeDto>()
                .RuleFor(x => x.Name, f => f.Random.String2(10))
                .RuleFor(x => x.TypeName, f => AssetTypes.Home)
                .RuleFor(x => x.Value, f => f.Random.Int(1000, 10000))
                .RuleFor(x => x.HomeType, f => f.Random.String2(10))
                .RuleFor(x => x.LoanValue, f => Math.Round(f.Random.Decimal(150000, 1000000), 3))
                .RuleFor(x => x.MonthlyMortgage, f => Math.Round(f.Random.Decimal(1500, 2500), 3))
                .RuleFor(x => x.MortgageRate, f => Math.Round(f.Random.Decimal(1, 1), 3))
                .RuleFor(x => x.DownPayment, f => Math.Round(f.Random.Decimal(20000, 50000), 3))
                .RuleFor(x => x.AnnualInsurance, f => f.Random.Decimal(1000, 1500))
                .RuleFor(x => x.AnnualPropertyTax, f => f.Random.Decimal(1, 3))
                .RuleFor(x => x.ClosingCosts, f => f.Random.Decimal(10000, 20000))
                .RuleFor(x => x.IsRefinanced, f => false);
        }

        public static AssetSnapshotDto RandomAssetSnapshotDto(int assetId)
        {
            return FakerAssetSnapshotDto(assetId).Generate();
        }
        public static List<AssetSnapshotDto> RandomAssetSnapshotDtos(int assetId, int? n = null)
        {
            return FakerAssetSnapshotDto(assetId).Generate(n ?? GeneratedAmount);
        }
        private static Faker<AssetSnapshotDto> FakerAssetSnapshotDto(int assetId)
        {
            var validAssetTypes = _Fake.GetFakeAssetTypes()
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            return new Faker<AssetSnapshotDto>()
                .RuleFor(x => x.AssetId, f => assetId)
                .RuleFor(x => x.Name, f => f.Random.String2(10))
                .RuleFor(x => x.TypeName, f => f.Random.ListItem(validAssetTypes))
                .RuleFor(x => x.Value, f => f.Random.Int(1000, 10000));
        }

        public static DateTime RandomStartDate()
        {
            var randomDays = new Faker()
                .Random
                .Int(5, 365);

            return DateTime.UtcNow.AddDays(-randomDays);
        }
        public static DateTime RandomEndDate(DateTime? startDate)
        {
            var endDate = new DateTime();

            if (startDate != null)
            {
                var nnStartDate = (DateTime)startDate;
                var days = new Faker()
                    .Random
                    .Int(1, 730);

                endDate = new DateTime(nnStartDate.Year, nnStartDate.Month, nnStartDate.Day).AddDays(days);
            }
            else
            {
                var randomDays = new Faker()
                    .Random
                    .Int(5, 365);

                endDate = DateTime.UtcNow.AddDays(-randomDays);
            }

            return endDate;
        }
        #endregion
    }
}
