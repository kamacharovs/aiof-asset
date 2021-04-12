using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace aiof.asset.data
{
    public class AssetType : IAssetType
    {
        public string Name { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
    }
}
