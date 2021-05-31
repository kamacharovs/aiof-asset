using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using FluentValidation;

using aiof.asset.data;

namespace aiof.asset.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class AssetValidatorTests
    {
        private readonly AbstractValidator<string> _typeValidator;
        private readonly AbstractValidator<AssetDto> _dtoValidator;
        private readonly AbstractValidator<AssetStockDto> _stockDtoValidator;
        private readonly AbstractValidator<AssetSnapshotDto> _snapshotValidator;

        private const int _defaultAssetId = 1;

        public AssetValidatorTests()
        {
            var service = new ServiceHelper();

            _typeValidator = service.GetRequiredService<AbstractValidator<string>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<string>));
            _dtoValidator = service.GetRequiredService<AbstractValidator<AssetDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<AssetDto>));
            _stockDtoValidator = service.GetRequiredService<AbstractValidator<AssetStockDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<AssetStockDto>));
            _snapshotValidator = service.GetRequiredService<AbstractValidator<AssetSnapshotDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<AssetSnapshotDto>));
        }

        #region AssetType
        [Theory]
        [MemberData(nameof(Helper.ValidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetType_Validation_IsValid(string type)
        {
            Assert.True((await _typeValidator.ValidateAsync(type)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetType_Validation_IsInvalid(string type)
        {
            Assert.False((await _typeValidator.ValidateAsync(type)).IsValid);
        }
        #endregion

        #region AssetDto
        [Theory]
        [MemberData(nameof(Helper.ValidNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Add_Validation_Name_IsValid(string name)
        {
            var dto = Helper.RandomAssetDto();

            dto.Name = name;

            Assert.True((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Add_Validation_Name_IsInvalid(string name)
        {
            var dto = Helper.RandomAssetDto();

            dto.Name = name;

            Assert.False((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Add_Validation_TypeName_IsValid(string typeName)
        {
            var dto = Helper.RandomAssetDto();

            dto.TypeName = typeName;

            Assert.True((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Add_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = Helper.RandomAssetDto();

            dto.TypeName = typeName;

            Assert.False((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidValues), MemberType = typeof(Helper))]
        public async Task AssetDto_Add_Validation_Value_IsValid(decimal value)
        {
            var dto = Helper.RandomAssetDto();

            dto.Value = value;

            Assert.True((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidValues), MemberType = typeof(Helper))]
        public async Task AssetDto_Add_Validation_Value_IsInvalid(decimal value)
        {
            var dto = Helper.RandomAssetDto();

            dto.Value = value;

            Assert.False((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Update_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = new AssetDto 
            {
                TypeName = typeName
            };

            Assert.False((await _dtoValidator.ValidateUpdateAssetAsync(dto)).IsValid);
        }

        [Fact]
        public async Task AssetDto_Add_Update_Validation_AllNull_IsInvalid()
        {
            var dto = new AssetDto { };

            Assert.False((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
            Assert.False((await _dtoValidator.ValidateUpdateAssetAsync(dto)).IsValid);
        }
        #endregion

        #region AssetStockDto
        [Theory]
        [MemberData(nameof(Helper.ValidNames), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_Name_IsValid(string name)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.Name = name;

            Assert.True((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidNames), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_Name_IsInvalid(string name)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.Name = name;

            Assert.False((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_TypeName_IsValid(string typeName)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.TypeName = typeName;

            Assert.True((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.TypeName = typeName;

            Assert.False((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidTickerSymbols), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_TickerSymbol_IsValid(string tickerSymbol)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.TickerSymbol = tickerSymbol;

            Assert.True((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidTickerSymbols), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_TickerSymbol_IsInvalid(string tickerSymbol)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.TickerSymbol = tickerSymbol;

            Assert.False((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidShares), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_Shares_IsValid(double shares)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.Shares = shares;

            Assert.True((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidShares), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_Shares_IsInvalid(double shares)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.Shares = shares;

            Assert.False((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidExpenseRatios), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_ExpenseRatio_IsValid(double expenseRatio)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.ExpenseRatio = expenseRatio;

            Assert.True((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidExpenseRatios), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_ExpenseRatio_IsInvalid(double expenseRatio)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.ExpenseRatio = expenseRatio;

            Assert.False((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidExpenseRatios), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_DividendYield_IsValid(double dividendYield)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.DividendYield = dividendYield;

            Assert.True((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidExpenseRatios), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Add_Validation_DividendYield_IsInvalid(double dividendYield)
        {
            var dto = Helper.RandomAssetStockDto();

            dto.DividendYield = dividendYield;

            Assert.False((await _stockDtoValidator.ValidateAddStockAsync(dto)).IsValid);
        }

        [Fact]
        public async Task AssetStockDto_Add_Validation_AllNull_IsInvalid()
        {
            var dto = new AssetStockDto { };

            Assert.False((await _dtoValidator.ValidateAddAssetAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetStockDto_Update_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = new AssetStockDto
            {
                TypeName = typeName
            };

            Assert.False((await _stockDtoValidator.ValidateUpdateStockAsync(dto)).IsValid);
        }
        #endregion

        #region AssetSnapshotDto
        [Theory]
        [MemberData(nameof(Helper.ValidNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Add_Validation_Name_IsValid(string name)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.Name = name;

            Assert.True((await _snapshotValidator.ValidateAddSnapshotAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Add_Validation_Name_IsInvalid(string name)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.Name = name;

            Assert.False((await _snapshotValidator.ValidateAddSnapshotAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Add_Validation_TypeName_IsValid(string typeName)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.TypeName = typeName;

            Assert.True((await _snapshotValidator.ValidateAddSnapshotAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Add_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.TypeName = typeName;

            Assert.False((await _snapshotValidator.ValidateAddSnapshotAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidValues), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Add_Validation_Value_IsInvalid(decimal value)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.Value = value;

            Assert.False((await _snapshotValidator.ValidateAddSnapshotAsync(dto)).IsValid);
        }
        
        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Update_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = new AssetSnapshotDto
            {
                AssetId = _defaultAssetId,
                TypeName = typeName
            };

            Assert.False((await _snapshotValidator.ValidateUpdateSnapshotAsync(dto)).IsValid);
        }
        #endregion
    }
}
