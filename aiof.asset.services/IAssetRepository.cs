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
        Task<IAsset> GetAsync(
            int id,
            bool asNoTracking = true);
        Task<IAsset> GetWithSnapshotsAsync(
            int id,
            DateTime? snapshotStartDate = null,
            DateTime? snapshotEndDate = null,
            bool asNoTracking = true);
    }
}
