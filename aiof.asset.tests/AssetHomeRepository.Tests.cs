using System;
using System.Threading.Tasks;
using System.Linq;

using Xunit;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class AssetHomeRepositoryTests
    {
        [Theory]
        [MemberData(nameof(Helper.AssetsUserId), MemberType = typeof(Helper))]
        public async Task AddAsync_Home_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetHomeRepository>();

            var dto = Helper.RandomAssetHomeDto();
            var asset = await repo.AddAsync(dto) as AssetHome;

            Assert.NotNull(asset);
            Assert.NotEqual(0, asset.Id);
            Assert.NotEqual(Guid.Empty, asset.PublicKey);
            Assert.Equal(dto.Name, asset.Name);
            Assert.Equal(AssetTypes.Home, asset.TypeName);
            Assert.NotNull(asset.Type);
            Assert.NotEqual(0, asset.Value);
            Assert.Equal(userId, asset.UserId);
            Assert.True(asset.Created > new DateTime());
            Assert.False(asset.IsDeleted);
            Assert.NotEmpty(asset.Snapshots);
            Assert.Equal(dto.HomeType, asset.HomeType);
            Assert.Equal(dto.LoanValue, asset.LoanValue);
            Assert.Equal(dto.MonthlyMortgage, asset.MonthlyMortgage);
            Assert.Equal(dto.MortgageRate, asset.MortgageRate);
            Assert.Equal(dto.DownPayment, asset.DownPayment);
            Assert.Equal(dto.AnnualInsurance, asset.AnnualInsurance);
            Assert.Equal(dto.AnnualPropertyTax, asset.AnnualPropertyTax);
            Assert.Equal(dto.ClosingCosts, asset.ClosingCosts);
            Assert.False(asset.IsRefinanced);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsHomeIdUserId), MemberType = typeof(Helper))]
        public async Task UpdateAsync_Home_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetHomeRepository>();

            var dto = Helper.RandomAssetHomeDto();

            var asset = await repo.UpdateAsync(id, dto) as AssetHome;

            Assert.NotNull(asset);
            Assert.Equal(id, asset.Id);
            Assert.Equal(dto.Name, asset.Name);
            Assert.Equal(dto.TypeName, asset.TypeName);
            Assert.Equal(dto.Value, asset.Value);
            Assert.Equal(dto.HomeType, asset.HomeType);
            Assert.Equal(dto.LoanValue, asset.LoanValue);
            Assert.Equal(dto.MonthlyMortgage, asset.MonthlyMortgage);
            Assert.Equal(dto.MortgageRate, asset.MortgageRate);
            Assert.Equal(dto.DownPayment, asset.DownPayment);
            Assert.Equal(dto.AnnualInsurance, asset.AnnualInsurance);
            Assert.Equal(dto.AnnualPropertyTax, asset.AnnualPropertyTax);
            Assert.Equal(dto.ClosingCosts, asset.ClosingCosts);
            Assert.False(asset.IsRefinanced);
            Assert.NotNull(asset.Snapshots.First().ValueChange);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsHomeIdUserId), MemberType = typeof(Helper))]
        public async Task DeleteAsync_Home_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();

            await repo.DeleteAsync(id);

            var exception = await Assert.ThrowsAsync<AssetNotFoundException>(() => repo.GetAsync(id));

            Assert.NotNull(exception);
            Assert.Equal(404, exception.StatusCode);
        }
    }
}
