using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.asset.data
{
    public class AssetHome : Asset
    {
        public string HomeType { get; set; }
        public double LoanValue { get; set; }
        public double MonthlyMortgage { get; set; }
        public double MortgageRate { get; set; }
        public double DownPayment { get; set; }
        public double? AnnualInsurance { get; set; }
        public double? AnnualPropertyTax { get; set; }
        public double? ClosingCosts { get; set; }
        public bool? IsRefinanced { get; set; } = false;

        public AssetHome()
        {
            TypeName = AssetTypes.Home;
        }
    }

    public class AssetHomeDto : AssetDto
    {
        public string HomeType { get; set; }
        public double? LoanValue { get; set; }
        public double? MonthlyMortgage { get; set; }
        public double? MortgageRate { get; set; }
        public double? DownPayment { get; set; }
        public double? AnnualInsurance { get; set; }
        public double? AnnualPropertyTax { get; set; }
        public double? ClosingCosts { get; set; }
        public bool? Refinanced { get; set; }
    }
}