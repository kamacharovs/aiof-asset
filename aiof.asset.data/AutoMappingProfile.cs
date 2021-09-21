using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

using AutoMapper;

namespace aiof.asset.data
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            /*
             * Asset
             */
            CreateMap<AssetDto, Asset>()
                .ForMember(x => x.Name, o => o.Condition(s => s.Name != null))
                .ForMember(x => x.TypeName, o => o.Condition(s => s.TypeName != null))
                .ForMember(x => x.Value, o => o.Condition(s => s.Value != null));

            CreateMap<AssetSnapshotDto, AssetSnapshot>()
                .ForMember(x => x.AssetId, o => o.MapFrom(s => s.AssetId))
                .ForMember(x => x.Name, o => o.Condition(s => s.Name != null))
                .ForMember(x => x.TypeName, o => o.Condition(s => s.TypeName != null))
                .ForMember(x => x.Value, o => o.Condition(s => s.Value != null));

            CreateMap<AssetDto, AssetSnapshotDto>()
                .ForMember(x => x.Name, o => o.Condition(s => s.Name != null))
                .ForMember(x => x.TypeName, o => o.Condition(s => s.TypeName != null))
                .ForMember(x => x.Value, o => o.Condition(s => s.Value != null));

            CreateMap<Asset, AssetSnapshotDto>()
                .ForMember(x => x.AssetId, o => o.MapFrom(s => s.Id))
                .ForMember(x => x.Name, o => o.MapFrom(s => s.Name))
                .ForMember(x => x.TypeName, o => o.MapFrom(s => s.TypeName))
                .ForMember(x => x.Value, o => o.MapFrom(s => s.Value));

            /*
             * Asset.Stock
             */
            CreateMap<AssetStockDto, AssetStock>()
                .ForMember(x => x.Name, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.Name)))
                .ForMember(x => x.TypeName, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.TypeName)))
                .ForMember(x => x.Value, o => o.Condition(s => s.Value.HasValue))
                .ForMember(x => x.TickerSymbol, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.TickerSymbol)))
                .ForMember(x => x.Shares, o => o.Condition(s => s.Shares.HasValue))
                .ForMember(x => x.ExpenseRatio, o => o.Condition(s => s.ExpenseRatio.HasValue))
                .ForMember(x => x.DividendYield, o => o.Condition(s => s.DividendYield.HasValue));

            CreateMap<AssetStockDto, AssetSnapshotDto>()
                .ForMember(x => x.Name, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.Name)))
                .ForMember(x => x.TypeName, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.TypeName)))
                .ForMember(x => x.Value, o => o.Condition(s => s.Value.HasValue));

            /*
             * Asset.Home
             */
            CreateMap<AssetHomeDto, AssetHome>()
                .ForMember(x => x.Name, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.Name)))
                .ForMember(x => x.TypeName, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.TypeName)))
                .ForMember(x => x.Value, o => o.Condition(s => s.Value.HasValue))
                .ForMember(x => x.HomeType, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.HomeType)))
                .ForMember(x => x.LoanValue, o => o.Condition(s => s.LoanValue.HasValue))
                .ForMember(x => x.MonthlyMortgage, o => o.Condition(s => s.MonthlyMortgage.HasValue))
                .ForMember(x => x.MortgageRate, o => o.Condition(s => s.MortgageRate.HasValue))
                .ForMember(x => x.DownPayment, o => o.Condition(s => s.DownPayment.HasValue))
                .ForMember(x => x.AnnualInsurance, o => o.Condition(s => s.AnnualInsurance.HasValue))
                .ForMember(x => x.AnnualPropertyTax, o => o.Condition(s => s.AnnualPropertyTax.HasValue))
                .ForMember(x => x.ClosingCosts, o => o.Condition(s => s.ClosingCosts.HasValue))
                .ForMember(x => x.IsRefinanced, o => o.Condition(s => s.IsRefinanced.HasValue));

            CreateMap<AssetHomeDto, AssetSnapshotDto>()
                .ForMember(x => x.Name, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.Name)))
                .ForMember(x => x.TypeName, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.TypeName)))
                .ForMember(x => x.Value, o => o.Condition(s => s.Value.HasValue));

        }
    }

    public class AssetEventProfile : Profile
    {
        public AssetEventProfile()
        {
            /*
             * AssetEvent
             */
            CreateMap<Tenant, EventUser>()
                .ForMember(x => x.Id, o => o.MapFrom(s => s.UserId))
                .ForMember(x => x.PublicKey, o => o.MapFrom(s => s.PublicKey));

            CreateMap<Asset, EventEntity>()
                .ForMember(x => x.Id, o => o.MapFrom(s => s.Id))
                .ForMember(x => x.Type, o => o.MapFrom(s => s.GetType().Name))
                .ForMember(x => x.Payload, o => o.MapFrom(s => s));
        }
    }
}
