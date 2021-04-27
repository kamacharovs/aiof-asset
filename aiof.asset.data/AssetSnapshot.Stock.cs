using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aiof.asset.data
{
    public class AssetStockSnapshot : AssetSnapshot
    {
        public string TickerSymbol { get; set; }
        public double? Shares { get; set; }
        public double? ExpenseRatio { get; set; }
        public double? DividendYield { get; set; }
    }
}
