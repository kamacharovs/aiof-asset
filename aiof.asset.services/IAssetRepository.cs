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
            DateTime? snapshotsEndDate = null);

        Task<IAsset> AddAsync(AssetDto dto);

        Task<IAssetSnapshot> AddSnapshotAsync(AssetSnapshotDto dto);

        Task<IAsset> UpdateAsync(
            int id,
            AssetDto dto);

        Task DeleteAsync(int id);
    }
}
