using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public interface IAsset
    {
        [Required]
        int Id { get; set; }

        [Required]
        Guid PublicKey { get; set; }

        [Required]
        [MaxLength(100)]
        string Name { get; set; }

        [Required]
        [MaxLength(100)]
        string TypeName { get; set; }

        [JsonIgnore]
        AssetType Type { get; set; }

        [Required]
        decimal Value { get; set; }

        [JsonIgnore]
        int UserId { get; set; }

        [Required]
        DateTime Created { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        ICollection<AssetSnapshot> Snapshots { get; set; }
    }
}