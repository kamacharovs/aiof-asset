using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

            _context.AssetsStock
                .AddRange(GetFakeAssetsStock());

            _context.AssetsHome
                .AddRange(GetFakeAssetsHome());

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
                    Name = AssetTypes.Car
                },
                new AssetType
                {
                    Name = AssetTypes.Home
                },
                new AssetType
                {
                    Name = AssetTypes.Investment
                },
                new AssetType
                {
                    Name = AssetTypes.Stock
                },
                new AssetType
                {
                    Name = AssetTypes.Cash
                },
                new AssetType
                {
                    Name = AssetTypes.Other
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
                    TypeName = AssetTypes.Car,
                    Value = 14762.12M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddDays(-30),
                    IsDeleted = false
                },
                new Asset
                {
                    Id = 2,
                    Name = "house",
                    TypeName = AssetTypes.Home,
                    Value = 300000M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddYears(-5),
                    IsDeleted = false,
                },
                new Asset
                {
                    Id = 3,
                    Name = "hardcoded guid",
                    TypeName = AssetTypes.Investment,
                    Value = 999999M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddDays(1),
                    IsDeleted = false,
                },
                new Asset
                {
                    Id = 4,
                    Name = "asset",
                    TypeName = AssetTypes.Cash,
                    Value = 99M,
                    UserId = 2,
                    Created = DateTime.UtcNow.AddDays(1),
                    IsDeleted = false,
                }
            };
        }

        public IEnumerable<AssetStock> GetFakeAssetsStock()
        {
            return new List<AssetStock>
            {
                new AssetStock
                {
                    Id = 5,
                    Name = "asset.stock",
                    Value = 10500M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddYears(-1),
                    IsDeleted = false,
                    TickerSymbol = "VTSAX",
                    Shares = 149.658M,
                    ExpenseRatio = 0.040M,
                    DividendYield = 1.36M
                }
            };
        }

        public IEnumerable<AssetHome> GetFakeAssetsHome()
        {
            return new List<AssetHome>
            {
                new AssetHome
                {
                    Id = 6,
                    Name = "asset.home",
                    Value = 375000M,
                    UserId = 1,
                    Created = DateTime.UtcNow.AddDays(-1),
                    IsDeleted = false,
                    HomeType = "apartment",
                    LoanValue = 335000M,
                    MonthlyMortgage = 1756.23M,
                    MortgageRate = 2.625M,
                    DownPayment = 40000M,
                    AnnualInsurance = 1.25M,
                    AnnualPropertyTax = 1.01M,
                    ClosingCosts = 16000M
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
                    Name = "car",
                    TypeName = "car",
                    Value = 13762.12M,
                    ValueChange = 0,
                    Created = DateTime.UtcNow
                },
                new AssetSnapshot
                {
                    Id = 2,
                    AssetId = 2,
                    Name = "house",
                    TypeName = "house",
                    Value = 260550M,
                    ValueChange = 0,
                    Created = DateTime.UtcNow.AddYears(-4)
                },
                new AssetSnapshot
                {
                    Id = 3,
                    AssetId = 2,
                    Value = 270550M,
                    ValueChange = 10000M,
                    Created = DateTime.UtcNow.AddYears(-3)
                },
                new AssetSnapshot
                {
                    Id = 4,
                    AssetId = 2,
                    Value = 300000M,
                    ValueChange = 29450M,
                    Created = DateTime.UtcNow
                },
                new AssetSnapshot
                {
                    Id = 5,
                    AssetId = 3,
                    Name = "hardcoded guid",
                    TypeName = AssetTypes.Investment,
                    Value = 999999M,
                    ValueChange = 0,
                    Created = DateTime.UtcNow.AddDays(1)
                },
                new AssetSnapshot
                {
                    Id = 6,
                    AssetId = 4,
                    Name = "asset",
                    TypeName = AssetTypes.Cash,
                    Value = 99M,
                    ValueChange = 0,
                    Created = DateTime.UtcNow.AddDays(1)
                },
                new AssetSnapshot
                {
                    Id = 7,
                    AssetId = 5,
                    Name = "asset.stock",
                    TypeName = AssetTypes.Stock,
                    Value = 10500M,
                    ValueChange = 0,
                    Created = DateTime.UtcNow.AddYears(-1)
                },
                new AssetSnapshot
                {
                    Id = 8,
                    AssetId = 6,
                    Name = "asset.home",
                    TypeName = AssetTypes.Home,
                    Value = 375000M,
                    ValueChange = 0,
                    Created = DateTime.UtcNow.AddDays(-1)
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

        public IEnumerable<object[]> GetFakeAssetsStockData(
            bool id = false,
            bool userId = false,
            bool isDeleted = false)
        {
            var fakeAssetsStock = GetFakeAssetsStock()
                .Where(x => x.IsDeleted == isDeleted)
                .ToArray();

            var toReturn = new List<object[]>();

            if (id
                && userId)
            {
                foreach (var fakeAssetStock in fakeAssetsStock)
                {
                    toReturn.Add(new object[]
                    {
                        fakeAssetStock.Id,
                        fakeAssetStock.UserId
                    });
                }
            }

            return toReturn;
        }

        public IEnumerable<object[]> GetFakeAssetsHomeData(
            bool id = false,
            bool userId = false,
            bool isDeleted = false)
        {
            var fakeAssetsHome = GetFakeAssetsHome()
                .Where(x => x.IsDeleted == isDeleted)
                .ToArray();

            var toReturn = new List<object[]>();

            if (id
                && userId)
            {
                foreach (var fakeAssetHome in fakeAssetsHome)
                {
                    toReturn.Add(new object[]
                    {
                        fakeAssetHome.Id,
                        fakeAssetHome.UserId
                    });
                }
            }

            return toReturn;
        }
    }
}
