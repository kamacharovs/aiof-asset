using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public interface IAssetSnapshot
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        int AssetId { get; set; }
        string Name { get; set; }
        string TypeName { get; set; }
        decimal? Value { get; set; }
        decimal? ValueChange { get; set; }
        DateTime Created { get; set; }
    }
}