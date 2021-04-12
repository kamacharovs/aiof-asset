using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace aiof.asset.data
{
    public interface IAssetType
    {
        string Name { get; set; }
        Guid PublicKey { get; set; }
    }
}