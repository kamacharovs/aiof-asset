using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using FluentValidation;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class AssetRepositoryTests
    {
        [Fact]
        public async Task GetTypesAsync_IsSuccessful()
        {
            var repo = new ServiceHelper().GetRequiredService<IAssetRepository>();
            var types = await repo.GetTypesAsync();

            Assert.NotNull(types);

            var type = types.FirstOrDefault();

            Assert.NotNull(type);
            Assert.NotNull(type.Name);
            Assert.NotEqual(Guid.Empty, type.PublicKey);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();
            var asset = await repo.GetAsync(id);

            Assert.NotNull(asset);
            Assert.NotEqual(0, asset.Id);
            Assert.NotEqual(Guid.Empty, asset.PublicKey);
            Assert.NotNull(asset.Name);
            Assert.NotNull(asset.TypeName);
            Assert.NotNull(asset.Type);
            Assert.NotEqual(0, asset.Value);
            Assert.NotEqual(0, asset.UserId);
            Assert.NotEqual(new DateTime(), asset.Created);
            Assert.False(asset.IsDeleted);

            var snapshots = asset.Snapshots;
            var snapshot = snapshots?.FirstOrDefault();

            if (snapshot is not null)
            {
                Assert.NotNull(snapshots);
                Assert.NotEqual(0, snapshot.Id);
                Assert.NotEqual(Guid.Empty, snapshot.PublicKey);
                Assert.NotEqual(0, snapshot.AssetId);
                Assert.True(snapshot.Name != null || snapshot.TypeName != null || snapshot.Value != null);
                Assert.NotEqual(new DateTime(), snapshot.Created);
            }
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_NotFound_ThrowsAssetNotFoundException(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var exception = await Assert.ThrowsAsync< AssetNotFoundException>(() => repo.GetAsync(id * 1000));

            Assert.NotNull(exception);
            Assert.Equal(404, exception.StatusCode);
            Assert.Contains("not found", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_WithRandomSnapshotsStartEndDates_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();
            var snapshotsStartDate = Helper.RandomStartDate();
            var snapshotsEndDate = Helper.RandomEndDate(snapshotsStartDate);

            var asset = await repo.GetAsync(id, snapshotsStartDate, snapshotsEndDate);

            Assert.NotNull(asset);

            var snapshots = asset.Snapshots;
            var snapshot = snapshots?.FirstOrDefault();

            if (snapshot is not null)
            {
                Assert.NotNull(snapshots);
                Assert.NotEqual(0, snapshot.Id);
                Assert.NotEqual(Guid.Empty, snapshot.PublicKey);
                Assert.NotEqual(0, snapshot.AssetId);
                Assert.True(snapshot.Name != null || snapshot.TypeName != null || snapshot.Value != null);
                Assert.NotEqual(new DateTime(), snapshot.Created);
            }
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();
            var asset = await repo.AddAsync(dto);

            Assert.NotNull(asset);
            Assert.NotEqual(0, asset.Id);
            Assert.NotEqual(Guid.Empty, asset.PublicKey);

            var snapshots = asset.Snapshots;
            var snapshot = snapshots?.FirstOrDefault();

            Assert.NotNull(snapshots);
            Assert.NotEmpty(snapshots);
            Assert.NotEqual(0, snapshot.Id);
            Assert.NotEqual(Guid.Empty, snapshot.PublicKey);
            Assert.NotEqual(0, snapshot.AssetId);
            Assert.NotNull(snapshot.Name);
            Assert.NotNull(snapshot.TypeName);
            Assert.NotNull(snapshot.Value);
            Assert.NotEqual(new DateTime(), snapshot.Created);
            Assert.Equal(dto.Name, snapshot.Name);
            Assert.Equal(dto.TypeName, snapshot.TypeName);
            Assert.Equal(dto.Value, snapshot.Value);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_TypeName_Empty_ThrowsValidationError(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            dto.TypeName = string.Empty;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => repo.AddAsync(dto));

            Assert.NotNull(exception);
            Assert.Equal(nameof(AssetDto.TypeName), exception.Errors.First().PropertyName);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Name_Empty_ThrowsValidationError(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            dto.Name = string.Empty;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => repo.AddAsync(dto));

            Assert.NotNull(exception);
            Assert.Equal(nameof(AssetDto.Name), exception.Errors.First().PropertyName);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Value_Negative_ThrowsValidationError(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            dto.Value = -1M;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => repo.AddAsync(dto));

            Assert.NotNull(exception);
            Assert.Equal(nameof(AssetDto.Value), exception.Errors.First().PropertyName);
        }
        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Value_TooBig_ThrowsValidationError(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            dto.Value = CommonValidator.MaximumValue + 1;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => repo.AddAsync(dto));

            Assert.NotNull(exception);
            Assert.Equal(nameof(AssetDto.Value), exception.Errors.First().PropertyName);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            var asset = await repo.UpdateAsync(id, dto);

            Assert.NotNull(asset);
            Assert.Equal(id, asset.Id);
            Assert.Equal(dto.Name, asset.Name);
            Assert.Equal(dto.TypeName, asset.TypeName);
            Assert.Equal(dto.Value, asset.Value);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_NotFound_ThrowsAssetNotFoundException(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var exception = await Assert.ThrowsAsync<AssetNotFoundException>(() => repo.UpdateAsync(id * 1000, Helper.RandomAssetDto()));

            Assert.NotNull(exception);
            Assert.Equal(404, exception.StatusCode);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_Name_Empty_ThrowsValidationError(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            dto.Name = string.Empty;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => repo.UpdateAsync(id, dto));

            Assert.NotNull(exception);
            Assert.Equal(nameof(AssetDto.Name), exception.Errors.First().PropertyName);
        }
        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_Value_Negative_ThrowsValidationError(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            dto.Value = -1M;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => repo.UpdateAsync(id, dto));

            Assert.NotNull(exception);
            Assert.Equal(nameof(AssetDto.Value), exception.Errors.First().PropertyName);
        }
        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_Value_TooBig_ThrowsValidationError(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetDto();

            dto.Value = CommonValidator.MaximumValue + 1;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => repo.UpdateAsync(id, dto));

            Assert.NotNull(exception);
            Assert.Equal(nameof(AssetDto.Value), exception.Errors.First().PropertyName);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task AddSnapshotAsync_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetSnapshotDto(id);
            var snapshot = await repo.AddSnapshotAsync(dto);

            Assert.NotNull(snapshot);
            Assert.NotEqual(0, snapshot.Id);
            Assert.NotEqual(Guid.Empty, snapshot.PublicKey);
            Assert.NotEqual(0, snapshot.AssetId);
            Assert.Equal(id, snapshot.AssetId);
            Assert.True(snapshot.Name != null || snapshot.TypeName != null || snapshot.Value != null);
            Assert.NotEqual(new DateTime(), snapshot.Created);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task DeleteAsync_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            await repo.DeleteAsync(id);

            var exception = await Assert.ThrowsAsync<AssetNotFoundException>(() => repo.GetAsync(id));

            Assert.NotNull(exception);
            Assert.Equal(404, exception.StatusCode);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task DeleteAsync_NotFound_ThrowsAssetNotFoundException(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var exception = await Assert.ThrowsAsync<AssetNotFoundException>(() => repo.DeleteAsync(id * 1000));

            Assert.NotNull(exception);
            Assert.Equal(404, exception.StatusCode);
        }
    }
}
