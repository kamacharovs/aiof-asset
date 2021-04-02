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

            _context.SaveChanges();
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
                    Name = "hardcoded guid",
                    TypeName = "investment",
                    Value = 999999M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddDays(1),
                    IsDeleted = false,
                },
                new Asset
                {
                    Id = 4,
                    Name = "asset",
                    TypeName = "cash",
                    Value = 99M,
                    UserId = 2,
                    Created = DateTime.UtcNow.AddDays(1),
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
                },
                new AssetSnapshot
                {
                    Id = 5,
                    AssetId = 3,
                    Name = "hardcoded guid",
                    TypeName = "investment",
                    Value = 999999M,
                    Created = DateTime.UtcNow.AddDays(1)
                },
                new AssetSnapshot
                {
                    Id = 6,
                    AssetId = 4,
                    Name = "asset",
                    TypeName = "cash",
                    Value = 99M,
                    Created = DateTime.UtcNow.AddDays(1)
                }
            };
        }

        public IEnumerable<object[]> GetFakeAssetsData(
            bool id = false,
            bool typeName = false,
            bool userId = false,
            bool isDeleted = false)
        {
            var fakeAssets = GetFakeAssets()
                .Where(x => x.IsDeleted == isDeleted)
                .ToArray();

            var toReturn = new List<object[]>();

            if (id
                && userId)
            {
                foreach (var fakeAsset in fakeAssets)
                {
                    toReturn.Add(new object[]
                    {
                        fakeAsset.Id,
                        fakeAsset.UserId
                    });
                }
            }
            else if (typeName
                && userId)
            {
                foreach (var fakeAsset in fakeAssets)
                {
                    toReturn.Add(new object[]
                    {
                        fakeAsset.TypeName,
                        fakeAsset.UserId
                    });
                }
            }
            else if (id)
            {
                foreach (var fakeAssetId in fakeAssets.Select(x => x.Id))
                {
                    toReturn.Add(new object[]
                    {
                        fakeAssetId
                    });
                }
            }
            else if (typeName)
            {
                foreach (var fakeAssetTypeName in fakeAssets.Select(x => x.TypeName).Distinct())
                {
                    toReturn.Add(new object[]
                    {
                        fakeAssetTypeName
                    });
                }
            }
            else if (userId)
            {
                foreach (var fakeAssetUserId in fakeAssets.Select(x => x.UserId).Distinct())
                {
                    toReturn.Add(new object[]
                    {
                        fakeAssetUserId
                    });
                }
            }

            return toReturn;
        }
    }
}
