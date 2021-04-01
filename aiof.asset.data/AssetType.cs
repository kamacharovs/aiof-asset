using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class AssetType : IAssetType
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [JsonIgnore]
        [Required]
        public Guid PublicKey { get; set; } = Guid.NewGuid();
    }
}
