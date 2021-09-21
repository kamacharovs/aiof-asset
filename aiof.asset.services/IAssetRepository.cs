using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aiof.asset.data;

namespace aiof.asset.services
{
    public interface IAssetRepository
    {
        Task<IEnumerable<IAssetType>> GetTypesAsync();

        Task<IAsset> GetAsync(
            int id,
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null);
        Task<IEnumerable<IAsset>> GetAsync(
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null,
            string type = null);
        Task<IAssetSnapshot> GetLatestSnapshotAsync(int assetId);
        Task<IAssetSnapshot> GetLatestSnapshotWithValueAsync(int assetId);

        Task<IAsset> AddAsync(AssetDto dto);
        Task<IEnumerable<IAsset>> AddAsync(IEnumerable<AssetDto> dtos);

        Task<IAssetSnapshot> AddSnapshotAsync(AssetSnapshotDto dto);

        Task<IAsset> UpdateAsync(
            int id,
            AssetDto dto);

        Task DeleteAsync(int id);
    }
}
