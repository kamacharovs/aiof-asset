using System;
using System.Threading.Tasks;
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
        #region GetTypesAsync
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
        #endregion

        #region GetAsync
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
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_All_IsSuccessfull(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();
            var assets = await repo.GetAsync();

            Assert.NotNull(assets);
            Assert.NotEmpty(assets);

            foreach (var asset in assets)
            {
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
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_NotFound_ThrowsAssetNotFoundException(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var exception = await Assert.ThrowsAsync<AssetNotFoundException>(() => repo.GetAsync(id * 1000));

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
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_WithLatestSnapshot_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();
            var snapshotsStartDate = DateTime.UtcNow.AddHours(12);
            var snapshotsEndDate = snapshotsStartDate.AddHours(24);

            var asset = await repo.GetAsync(id);

            Assert.NotNull(asset);

            var snapshots = asset.Snapshots;

            Assert.Single(snapshots);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_Snapshots_EndDate_SmallerThan_StartDate_ThrowsBadRequest(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();
            var snapshotsStartDate = Helper.RandomStartDate();
            var snapshotsEndDate = snapshotsStartDate.AddDays(-1);

            var exception = await Assert.ThrowsAsync<AssetFriendlyException>(() => repo.GetAsync(id, snapshotsStartDate, snapshotsEndDate));

            Assert.NotNull(exception);
            Assert.Equal(400, exception.StatusCode);
            Assert.Contains("end date cannot be earlier than start date", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region AddAsync
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
            Assert.Equal(0, snapshot.ValueChange);
            Assert.NotEqual(new DateTime(), snapshot.Created);
            Assert.Equal(dto.Name, snapshot.Name);
            Assert.Equal(dto.TypeName, snapshot.TypeName);
            Assert.Equal(dto.Value, snapshot.Value);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Multiple_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dtos = Helper.RandomAssetDtos(3);
            var assets = await repo.AddAsync(dtos);

            Assert.NotNull(assets);
            Assert.NotEmpty(assets);

            var asset = assets.FirstOrDefault();

            Assert.NotNull(asset);
            Assert.NotEqual(0, asset.Id);
            Assert.NotEqual(Guid.Empty, asset.PublicKey);
            Assert.NotNull(asset.Name);
            Assert.NotNull(asset.TypeName);
            Assert.NotNull(asset.Type);
            Assert.NotEqual(0, asset.Value);
            Assert.NotEqual(new DateTime(), asset.Created);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Stock_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetStockDto();
            var asset = await repo.AddAsync(dto) as AssetStock;

            Assert.NotNull(asset);
            Assert.NotEqual(0, asset.Id);
            Assert.NotEqual(Guid.Empty, asset.PublicKey);
            Assert.Equal(dto.Name, asset.Name);
            Assert.Equal(AssetTypes.Stock, asset.TypeName);
            Assert.NotNull(asset.Type);
            Assert.NotEqual(0, asset.Value);
            Assert.Equal(userId, asset.UserId);
            Assert.False(asset.IsDeleted);
            Assert.NotEmpty(asset.Snapshots);
            Assert.Equal(dto.TickerSymbol, asset.TickerSymbol);
            Assert.Equal(dto.Shares, asset.Shares);
            Assert.Equal(dto.ExpenseRatio, asset.ExpenseRatio);
            Assert.Equal(dto.DividendYield, asset.DividendYield);

            var snapshots = asset.Snapshots;
            var snapshot = snapshots?.FirstOrDefault();

            Assert.Equal(dto.Name, snapshot.Name);
            Assert.Equal(dto.TypeName, snapshot.TypeName);
            Assert.Equal(dto.Value, snapshot.Value);
            Assert.Equal(0, snapshot.ValueChange);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_UpdateAsync_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var value = 1500M;
            var valueToChange = 500M;

            var dto = Helper.RandomAssetDto();

            dto.Value = value;

            var asset = await repo.AddAsync(dto);

            Assert.NotNull(asset);
            Assert.NotEqual(0, asset.Id);
            Assert.NotEqual(Guid.Empty, asset.PublicKey);

            var updatedAsset = await repo.UpdateAsync(asset.Id,
                new AssetDto { Value = value + valueToChange });

            Assert.NotNull(updatedAsset);
            Assert.NotEqual(0, updatedAsset.Id);
            Assert.NotEqual(Guid.Empty, updatedAsset.PublicKey);

            var latestSnapshot = await repo.GetLatestSnapshotWithValueAsync(updatedAsset.Id);

            Assert.NotNull(latestSnapshot);
            Assert.NotNull(latestSnapshot.Value);
            Assert.NotNull(latestSnapshot.ValueChange);
            Assert.Equal(valueToChange, latestSnapshot.ValueChange);
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
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Multiple_OneValidationError_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dtos = Helper.RandomAssetDtos(2);

            dtos.First().Name = Helper.Repeat(dtos.First().Name, 100);

            var assets = await repo.AddAsync(dtos);

            Assert.NotNull(assets);
            Assert.NotEmpty(assets);
            Assert.True(assets.Count() == 1);
        }
        #endregion

        #region UpdateAsync
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
            Assert.NotNull(asset.Snapshots.First().ValueChange);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsStockIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_Stock_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var dto = Helper.RandomAssetStockDto();

            var asset = await repo.UpdateAsync(id, dto) as AssetStock;

            Assert.NotNull(asset);
            Assert.Equal(id, asset.Id);
            Assert.Equal(dto.Name, asset.Name);
            Assert.Equal(dto.TypeName, asset.TypeName);
            Assert.Equal(dto.Value, asset.Value);
            Assert.Equal(dto.TickerSymbol, asset.TickerSymbol);
            Assert.Equal(dto.Shares, asset.Shares);
            Assert.Equal(dto.ExpenseRatio, asset.ExpenseRatio);
            Assert.Equal(dto.DividendYield, asset.DividendYield);
            Assert.NotNull(asset.Snapshots.First().ValueChange);
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
        #endregion

        #region GetLatestSnapshotAsync
        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetLatestSnapshotAsync_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var snapshot = await repo.GetLatestSnapshotAsync(id);

            Assert.NotNull(snapshot);
            Assert.True(snapshot.ValueChange == 0 || snapshot.ValueChange < 0 || snapshot.ValueChange > 0);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetLatestSnapshotAsync_DoesntExist_IsNull(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var snapshot = await repo.GetLatestSnapshotAsync(id * 1000);

            Assert.Null(snapshot);
        }
        #endregion

        #region GetLatestSnapshotWithValueAsync
        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetLatestSnapshotWithValueAsync_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            var snapshot = await repo.GetLatestSnapshotWithValueAsync(id);

            Assert.NotNull(snapshot);
            Assert.True(snapshot.ValueChange == 0 || snapshot.ValueChange < 0 || snapshot.ValueChange > 0);
        }
        #endregion

        #region AddSnapshotAsync
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
        #endregion

        #region DeleteAsync
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
        [MemberData(nameof(Helper.AssetsStockIdUserId), MemberType = typeof(Helper))]
        public async Task DeleteAsync_Stock_IsSuccessful(int id, int userId)
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
        #endregion
    }
}
