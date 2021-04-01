using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

namespace aiof.asset.data
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
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
        }
    }
}
