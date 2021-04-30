using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class Asset : IAsset
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string TypeName { get; set; }
        public AssetType Type { get; set; }
        public decimal Value { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public bool IsDeleted { get; set; } = false;

        public ICollection<AssetSnapshot> Snapshots { get; set; } = new List<AssetSnapshot>();
    }

    public class AssetDto
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string TypeName { get; set; }

        public decimal? Value { get; set; }
    }
}
