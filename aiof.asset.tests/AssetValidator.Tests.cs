﻿using System;
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
        private readonly AbstractValidator<AssetSnapshotDto> _snapshotValidator;

        private const int _defaultAssetId = 1;

        public AssetValidatorTests()
        {
            var service = new ServiceHelper();

            _typeValidator = service.GetRequiredService<AbstractValidator<string>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<string>));
            _dtoValidator = service.GetRequiredService<AbstractValidator<AssetDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<AssetDto>));
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
        public async Task AssetDto_Validation_Name_IsValid(string name)
        {
            var dto = Helper.RandomAssetDto();

            dto.Name = name;

            Assert.True((await _dtoValidator.ValidateAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Validation_Name_IsInvalid(string name)
        {
            var dto = Helper.RandomAssetDto();

            dto.Name = name;

            Assert.False((await _dtoValidator.ValidateAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Validation_TypeName_IsValid(string typeName)
        {
            var dto = Helper.RandomAssetDto();

            dto.TypeName = typeName;

            Assert.True((await _dtoValidator.ValidateAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetDto_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = Helper.RandomAssetDto();

            dto.TypeName = typeName;

            Assert.False((await _dtoValidator.ValidateAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidValues), MemberType = typeof(Helper))]
        public async Task AssetDto_Validation_Value_IsValid(decimal value)
        {
            var dto = Helper.RandomAssetDto();

            dto.Value = value;

            Assert.True((await _dtoValidator.ValidateAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidValues), MemberType = typeof(Helper))]
        public async Task AssetDto_Validation_Value_IsInvalid(decimal value)
        {
            var dto = Helper.RandomAssetDto();

            dto.Value = value;

            Assert.False((await _dtoValidator.ValidateAsync(dto)).IsValid);
        }
        #endregion

        #region AssetSnapshotDto
        [Theory]
        [MemberData(nameof(Helper.ValidNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Validation_Name_IsValid(string name)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.Name = name;

            Assert.True((await _snapshotValidator.ValidateAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Validation_Name_IsInvalid(string name)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.Name = name;

            Assert.False((await _snapshotValidator.ValidateAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.ValidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Validation_TypeName_IsValid(string typeName)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.TypeName = typeName;

            Assert.True((await _snapshotValidator.ValidateAsync(dto)).IsValid);
        }
        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.TypeName = typeName;

            Assert.False((await _snapshotValidator.ValidateAsync(dto)).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidValues), MemberType = typeof(Helper))]
        public async Task AssetSnapshotDto_Validation_Value_IsInvalid(decimal value)
        {
            var dto = Helper.RandomAssetSnapshotDto(_defaultAssetId);

            dto.Value = value;

            Assert.False((await _snapshotValidator.ValidateAsync(dto)).IsValid);
        }
        #endregion
    }
}
