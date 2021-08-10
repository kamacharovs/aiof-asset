﻿using System;
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
                .ForAllMembers(x => x.Condition((source, destination, member) => member != null));

            CreateMap<AssetStockDto, AssetSnapshotDto>()
                .ForAllMembers(x => x.Condition((source, destination, member) => member != null));

            /*
             * Asset.Home
             */
            CreateMap<AssetHomeDto, AssetHome>()
                .ForAllMembers(x => x.Condition((source, destination, member) => member != null));

            CreateMap<AssetHomeDto, AssetSnapshotDto>()
                .ForAllMembers(x => x.Condition((source, destination, member) => member != null));

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
