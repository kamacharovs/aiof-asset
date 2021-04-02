using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;
using FluentValidation;
using Moq;

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
        #endregion
    }
}
