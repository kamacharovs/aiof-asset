using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace aiof.asset.data
{
    public interface IAsset
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        string Name { get; set; }
        string TypeName { get; set; }
        AssetType Type { get; set; }
        decimal Value { get; set; }
        int UserId { get; set; }
        DateTime Created { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        ICollection<AssetSnapshot> Snapshots { get; set; }
    }
}