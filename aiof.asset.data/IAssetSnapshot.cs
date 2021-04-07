using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public interface IAssetSnapshot
    {
        [Required]
        int Id { get; set; }

        [Required]
        Guid PublicKey { get; set; }

        int AssetId { get; set; }

        [MaxLength(100)]
        string Name { get; set; }

        [MaxLength(100)]
        string TypeName { get; set; }

        decimal? Value { get; set; }

        decimal? ValueChange { get; set; }

        [Required]
        DateTime Created { get; set; }
    }
}