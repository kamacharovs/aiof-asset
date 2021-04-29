using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;
using FluentValidation;
using Moq;
using Bogus;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.tests
{
    [ExcludeFromCodeCoverage]
    public class ServiceHelper
    {
        public int? UserId { get; set; }
        public int? ClientId { get; set; }

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
                .AddScoped<ITenant, Tenant>()
                .AddScoped<FakeDataManager>();
            services.AddScoped(x => GetMockTenant());
            services.AddSingleton(new MapperConfiguration(x => { x.AddProfile(new AutoMappingProfile()); }).CreateMapper());
            
            services.AddDbContext<AssetContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddScoped<AbstractValidator<string>, AssetTypeValidator>()
                .AddScoped<AbstractValidator<AssetDto>, AssetDtoValidator>()
                .AddScoped<AbstractValidator<AssetSnapshotDto>, AssetSnapshotDtoValidator>()
                .AddScoped<AbstractValidator<AssetStockDto>, AssetStockDtoValidator>();

            services.AddLogging();
            services.AddHttpContextAccessor();

            return services;
        }

        public ITenant GetMockTenant()
        {
            var mockedTenant = new Mock<ITenant>();
            var userId = UserId ?? 1;
            var clientId = ClientId ?? 1;

            mockedTenant.Setup(x => x.UserId).Returns(userId);
            mockedTenant.Setup(x => x.ClientId).Returns(clientId);

            return mockedTenant.Object;
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
                .RuleFor(x => x.Shares, f => Math.Round(f.Random.Double(10, 150), 2))
                .RuleFor(x => x.ExpenseRatio, f => Math.Round(f.Random.Double(0.001, 0.005), 5))
                .RuleFor(x => x.DividendYield, f => Math.Round(f.Random.Double(0.001, 0.05), 5));
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
                endDate = DateTime.UtcNow.AddDays(-startDate.Value.Day);
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
