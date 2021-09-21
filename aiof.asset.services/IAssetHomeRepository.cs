using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aiof.asset.data;

namespace aiof.asset.services
{
    public interface IAssetHomeRepository
    {
        Task<IAsset> AddAsync(AssetHomeDto dto);
        Task<IAsset> UpdateAsync(int id, AssetHomeDto dto);
    }
}
