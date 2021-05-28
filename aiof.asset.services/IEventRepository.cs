using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aiof.asset.data;

namespace aiof.asset.services
{
    public interface IEventRepository
    {
        Task EmitAsync<T>(Asset asset)
            where T : AssetEvent, new();
    }
}
