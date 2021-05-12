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
            DateTime? snapshotsEndDate = null,
            bool asNoTracking = true);
        Task<IEnumerable<IAsset>> GetAsync(
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null,
            string type = null,
            bool asNoTracking = true);
        Task<IAssetSnapshot> GetLatestSnapshotAsync(
            int assetId,
            bool asNoTracking = true);
        Task<IAssetSnapshot> GetLatestSnapshotWithValueAsync(
            int assetId,
            bool asNoTracking = true);

        Task<IAsset> AddAsync(AssetDto dto);
        Task<IEnumerable<IAsset>> AddAsync(IEnumerable<AssetDto> dtos);
        Task<IAsset> AddAsync(AssetStockDto dto);

        Task<IAssetSnapshot> AddSnapshotAsync(AssetSnapshotDto dto);

        Task<IAsset> UpdateAsync(
            int id,
            AssetDto dto);
        Task<IAsset> UpdateAsync(
            int id,
            AssetStockDto dto);

        Task DeleteAsync(int id);
    }
}
