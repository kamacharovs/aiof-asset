using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class AssetStock : Asset
    {
        public string TickerSymbol { get; set; }
        public double? Shares { get; set; }
        public double? ExpenseRatio { get; set; }
        public double? DividendYield { get; set; }

        public AssetStock()
        {
            TypeName = AssetTypes.Stock;
        }
    }

    public class AssetStockDto : AssetDto
    {
        [MaxLength(50)]
        public string TickerSymbol { get; set; }

        public double? Shares { get; set; }
        public double? ExpenseRatio { get; set; }
        public double? DividendYield { get; set; }

        public AssetStockDto()
        {
            TypeName = AssetTypes.Stock;
        }
    }
}