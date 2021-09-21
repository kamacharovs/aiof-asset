using System;
using System.Threading.Tasks;
using System.Linq;

using Xunit;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class AssetStockRepositoryTests
    {
        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Stock_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetStockRepository>();

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
        [MemberData(nameof(Helper.AssetsStockIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_Stock_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetStockRepository>();

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
        [MemberData(nameof(Helper.AssetsStockIdUserId), MemberType = typeof(Helper))]
        public async Task DeleteAsync_Stock_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            await repo.DeleteAsync(id);

            var exception = await Assert.ThrowsAsync<AssetNotFoundException>(() => repo.GetAsync(id));

            Assert.NotNull(exception);
            Assert.Equal(404, exception.StatusCode);
        }
    }
}
