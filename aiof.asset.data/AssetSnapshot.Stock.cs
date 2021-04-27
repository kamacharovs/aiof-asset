using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class AssetStockSnapshot : AssetSnapshot
    {
        public string TickerSymbol { get; set; }
        public double? Shares { get; set; }
        public double? ExpenseRatio { get; set; }
        public double? DividendYield { get; set; }
    }

    public class AssetStockSnapshotDto : AssetSnapshotDto
    {
        [MaxLength(50)]
        public string TickerSymbol { get; set; }

        public double? Shares { get; set; }
        public double? ExpenseRatio { get; set; }
        public double? DividendYield { get; set; }
    }
}
