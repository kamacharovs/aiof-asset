using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.tests
{
    public class AssetsRepositoryTests
    {
        [Theory]
        [MemberData(nameof(Helper.AssetsIdUserId), MemberType = typeof(Helper))]
        public async Task GetAsync_IsSuccessful(int id, int userId)
        {
            var _repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IAssetRepository>();
            var asset = await _repo.GetAsync(id);

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
        }
    }
}
