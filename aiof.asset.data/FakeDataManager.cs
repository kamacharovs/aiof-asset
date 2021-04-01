using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace aiof.asset.data
{
    [ExcludeFromCodeCoverage]
    public class FakeDataManager
    {
        private readonly AssetContext _context;

        public FakeDataManager(AssetContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void UseFakeContext()
        {
            _context.AssetTypes
                .AddRange(GetFakeAssetTypes());

            _context.Assets
                .AddRange(GetFakeAssets());

            _context.AssetSnapshots
                .AddRange(GetFakeAssetSnapshots());
        }

        public IEnumerable<AssetType> GetFakeAssetTypes()
        {
            return new List<AssetType>
            {
                new AssetType
                {
                    Name = "car"
                },
                new AssetType
                {
                    Name = "house"
                },
                new AssetType
                {
                    Name = "investment"
                },
                new AssetType
                {
                    Name = "stock"
                },
                new AssetType
                {
                    Name = "cash"
                },
                new AssetType
                {
                    Name = "other"
                }
            };
        }

        public IEnumerable<Asset> GetFakeAssets()
        {
            return new List<Asset>
            {
                new Asset
                {
                    Id = 1,
                    PublicKey = Guid.Parse("1ada5134-0290-4ec6-9933-53040906b255"),
                    Name = "car",
                    TypeName = "car",
                    Value = 14762.12M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddDays(-30),
                    IsDeleted = false
                },
                new Asset
                {
                    Id = 2,
                    PublicKey = Guid.Parse("242948e5-6760-43c6-b6ff-21c40de3f9af"),
                    Name = "house",
                    TypeName = "house",
                    Value = 250550M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddYears(-5),
                    IsDeleted = false,
                },
                new Asset
                {
                    Id = 3,
                    PublicKey = Guid.Parse("dbf79a48-0504-4bd0-ad00-8cbc3044e585"),
                    Name = "hardcoded guid",
                    TypeName = "investment",
                    Value = 999999M,
                    UserId = 1,
                    IsDeleted = false,
                },
                new Asset
                {
                    Id = 4,
                    PublicKey = Guid.Parse("97bedb5b-c49e-484a-8bd0-1d7cb474e217"),
                    Name = "asset",
                    TypeName = "cash",
                    Value = 99M,
                    UserId = 2,
                    IsDeleted = false,
                }
            };
        }

        public IEnumerable<AssetSnapshot> GetFakeAssetSnapshots()
        {
            return new List<AssetSnapshot>
            {
                new AssetSnapshot
                {
                    Id = 1,
                    AssetId = 1,
                    Value = 13762.12M,
                    Created = DateTime.UtcNow,
                },
                new AssetSnapshot
                {
                    Id = 2,
                    AssetId = 2,
                    Value = 260550M,
                    Created = DateTime.UtcNow.AddYears(-4)
                },
                new AssetSnapshot
                {
                    Id = 3,
                    AssetId = 2,
                    Value = 270550M,
                    Created = DateTime.UtcNow.AddYears(-3)
                },
                new AssetSnapshot
                {
                    Id = 4,
                    AssetId = 2,
                    Value = 300000M,
                    Created = DateTime.UtcNow
                }
            };
       }
    }
}
