using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class AssetStock : Asset
    {
        public string TickerSymbol { get; set; }
        public decimal? Shares { get; set; }
        public decimal? ExpenseRatio { get; set; }
        public decimal? DividendYield { get; set; }

        public AssetStock()
        {
            TypeName = AssetTypes.Stock;
        }
    }

    public class AssetStockDto : AssetDto
    {
        [MaxLength(50)]
        public string TickerSymbol { get; set; }

        public decimal? Shares { get; set; }
        public decimal? ExpenseRatio { get; set; }
        public decimal? DividendYield { get; set; }
    }
}