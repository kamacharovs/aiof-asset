using System;

namespace aiof.asset.data
{
    public class AssetType : IAssetType
    {
        public string Name { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
    }
}
