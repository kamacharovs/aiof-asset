using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aiof.asset.data
{
    public interface IEnvConfiguration
    {
        Task<bool> IsEnabledAsync(FeatureFlags featureFlag);
    }
}
