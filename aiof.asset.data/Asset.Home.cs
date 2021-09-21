using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class AssetHome : Asset
    {
        public string HomeType { get; set; }
        public decimal LoanValue { get; set; }
        public decimal MonthlyMortgage { get; set; }
        public decimal MortgageRate { get; set; }
        public decimal DownPayment { get; set; }
        public decimal? AnnualInsurance { get; set; }
        public decimal? AnnualPropertyTax { get; set; }
        public decimal? ClosingCosts { get; set; }
        public bool? IsRefinanced { get; set; } = false;

        public AssetHome()
        {
            TypeName = AssetTypes.Home;
        }
    }

    public class AssetHomeDto : AssetDto
    {
        [MaxLength(100)]
        public string HomeType { get; set; }

        public decimal? LoanValue { get; set; }
        public decimal? MonthlyMortgage { get; set; }
        public decimal? MortgageRate { get; set; }
        public decimal? DownPayment { get; set; }
        public decimal? AnnualInsurance { get; set; }
        public decimal? AnnualPropertyTax { get; set; }
        public decimal? ClosingCosts { get; set; }
        public bool? IsRefinanced { get; set; }
    }
}