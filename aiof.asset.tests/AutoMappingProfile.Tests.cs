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
    }
}
