﻿using System;
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

            services.AddSingleton<AbstractValidator<AssetDto>, AssetDtoValidator>()
                .AddSingleton<AbstractValidator<AssetSnapshotDto>, AssetSnapshotDtoValidator>();

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

        public static AssetSnapshotDto RandomAssetSnapshotDto()
        {
            return FakerAssetSnapshotDto().Generate();
        }
        public static List<AssetSnapshotDto> RandomAssetSnapshotDtos(int? n = null)
        {
            return FakerAssetSnapshotDto().Generate(n ?? GeneratedAmount);
        }
        private static Faker<AssetSnapshotDto> FakerAssetSnapshotDto()
        {
            var validAssetIds = _Fake.GetFakeAssetSnapshots()
                .Select(x => x.AssetId)
                .Distinct()
                .ToList();

            var validAssetTypes = _Fake.GetFakeAssetTypes()
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            return new Faker<AssetSnapshotDto>()
                .RuleFor(x => x.AssetId, f => f.Random.ListItem(validAssetIds))
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
