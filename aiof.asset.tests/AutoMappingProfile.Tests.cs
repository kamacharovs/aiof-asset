using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using AutoMapper;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class AutoMappingProfileTests
    {
        private readonly IMapper _mapper;

        private const int _defaultAssetId = 1;

        public AutoMappingProfileTests()
        {
            _mapper = new ServiceHelper().GetRequiredService<IMapper>() ?? throw new ArgumentNullException(nameof(IMapper));
        }

        [Fact]
        public void AssetDto_To_Asset_IsSuccessful()
        {
            var dto = Helper.RandomAssetDto();
            var asset = _mapper.Map<Asset>(dto);

            Assert.NotNull(asset);
            Assert.Equal(0, asset.Id);
            Assert.NotEqual(Guid.Empty, asset.PublicKey);
            Assert.Equal(dto.Name, asset.Name);
            Assert.Equal(dto.TypeName, asset.TypeName);
            Assert.Equal(dto.Value, asset.Value);
        }

        [Fact]
        public void AssetDto_To_AssetSnapshotDto_IsSuccessful()
        {
            var dto = Helper.RandomAssetDto();
            var snapshotDto = _mapper.Map<AssetSnapshotDto>(dto);

            Assert.NotNull(snapshotDto);
            Assert.Equal(0, snapshotDto.AssetId);
            Assert.Equal(dto.Name, snapshotDto.Name);
            Assert.Equal(dto.TypeName, snapshotDto.TypeName);
            Assert.Equal(dto.Value, snapshotDto.Value);
        }

        [Fact]
        public void AssetSnapshotDto_To_AssetSnapshot_IsSuccessful()
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);
            var snapshot = _mapper.Map<AssetSnapshot>(dto);

            Assert.NotNull(snapshot);
            Assert.Equal(0, snapshot.Id);
            Assert.NotEqual(Guid.Empty, snapshot.PublicKey);
            Assert.Equal(dto.AssetId, snapshot.AssetId);
            Assert.Equal(dto.Name, snapshot.Name);
            Assert.Equal(dto.TypeName, snapshot.TypeName);
            Assert.Equal(dto.Value, snapshot.Value);
        }

        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task Asset_To_AssetSnapshotDto_IsSuccessful(int id, int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();
            var asset = await repo.GetAsync(id);

            var snapshotDto = _mapper.Map<AssetSnapshotDto>(asset);

            Assert.NotNull(snapshotDto);
            Assert.Equal(asset.Id, snapshotDto.AssetId);
            Assert.Equal(asset.Name, snapshotDto.Name);
            Assert.Equal(asset.TypeName, snapshotDto.TypeName);
            Assert.Equal(asset.Value, snapshotDto.Value);
        }

        [Fact]
        public void AssetStockDto_To_AssetStock_IsSuccessful()
        {
            var assetStockDto = Helper.RandomAssetStockDto();
            var assetStock = _mapper.Map<AssetStock>(assetStockDto);

            Assert.NotNull(assetStock);
            Assert.NotNull(assetStock.Name);
            Assert.NotNull(assetStock.TypeName);
            Assert.True(assetStock.Value > 0);
            Assert.NotNull(assetStock.TickerSymbol);
            Assert.NotNull(assetStock.Shares);
            Assert.NotNull(assetStock.ExpenseRatio);
            Assert.NotNull(assetStock.DividendYield);
        }

        [Fact]
        public void AssetStockDto_To_AssetSnapshotDto_IsSuccessful()
        {
            var assetStockDto = Helper.RandomAssetStockDto();
            var assetSnapshotDto = _mapper.Map<AssetSnapshotDto>(assetStockDto);

            Assert.NotNull(assetSnapshotDto);
            Assert.NotNull(assetSnapshotDto.Name);
            Assert.NotNull(assetSnapshotDto.TypeName);
            Assert.NotNull(assetSnapshotDto.Value);
            Assert.Equal(assetSnapshotDto.Name, assetStockDto.Name);
            Assert.Equal(assetSnapshotDto.TypeName, assetStockDto.TypeName);
            Assert.Equal(assetSnapshotDto.Value, assetStockDto.Value);
        }

        [Fact]
        public void AssetHomeDto_To_AssetHome_IsSuccessful()
        {
            var assetHomeDto = Helper.RandomAssetHomeDto();
            var assetHome = _mapper.Map<AssetHome>(assetHomeDto);

            Assert.NotNull(assetHome);
            Assert.NotNull(assetHome.Name);
            Assert.NotNull(assetHome.TypeName);
            Assert.True(assetHome.Value > 0);
            Assert.Equal(assetHome.Name, assetHomeDto.Name);
            Assert.Equal(assetHome.TypeName, assetHomeDto.TypeName);
            Assert.Equal(assetHome.Value, assetHomeDto.Value);
            Assert.NotNull(assetHome.HomeType);
            Assert.True(assetHome.LoanValue > CommonValidator.MinimumValue);
            Assert.True(assetHome.MonthlyMortgage > CommonValidator.MinimumValue);
            Assert.True(assetHome.MortgageRate > CommonValidator.MinimumPercentValue);
            Assert.True(assetHome.DownPayment > CommonValidator.MinimumValue);
            Assert.True(assetHome.AnnualInsurance > CommonValidator.MinimumValue);
            Assert.True(assetHome.AnnualPropertyTax > CommonValidator.MinimumPercentValue);
            Assert.True(assetHome.ClosingCosts > CommonValidator.MinimumValue);
            Assert.False(assetHome.IsRefinanced);
        }

        [Fact]
        public void AssetHomeDto_To_AssetSnapshotDto_IsSuccessful()
        {
            var assetHomeDto = Helper.RandomAssetHomeDto();
            var assetSnapshotDto = _mapper.Map<AssetSnapshotDto>(assetHomeDto);

            Assert.NotNull(assetSnapshotDto);
            Assert.NotNull(assetSnapshotDto.Name);
            Assert.NotNull(assetSnapshotDto.TypeName);
            Assert.NotNull(assetSnapshotDto.Value);
            Assert.Equal(assetSnapshotDto.Name, assetHomeDto.Name);
            Assert.Equal(assetSnapshotDto.TypeName, assetHomeDto.TypeName);
            Assert.Equal(assetSnapshotDto.Value, assetHomeDto.Value);
        }
    }
}
