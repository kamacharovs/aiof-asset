using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class AssetSnapshot : IAssetSnapshot
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public int AssetId { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueChange { get; set; } = 0;
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public class AssetSnapshotDto
    {
        [Required]
        public int AssetId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string TypeName { get; set; }

        public decimal? Value { get; set; }
    }
}
