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
        private readonly AbstractValidator<AssetDto> _dtoValidator;
        private readonly AbstractValidator<AssetSnapshotDto> _snapshotValidator;

        public AssetValidatorTests()
        {
            var service = new ServiceHelper();

            _dtoValidator = service.GetRequiredService<AbstractValidator<AssetDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<AssetDto>));
            _snapshotValidator = service.GetRequiredService<AbstractValidator<AssetSnapshotDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<AssetSnapshotDto>));
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidNames), MemberType = typeof(Helper))]
        public void AssetDto_Validation_Name_IsInvalid(string name)
        {
            var dto = Helper.RandomAssetDto();

            dto.Name = name;

            Assert.False(_dtoValidator.Validate(dto).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidTypeNames), MemberType = typeof(Helper))]
        public void AssetDto_Validation_TypeName_IsInvalid(string typeName)
        {
            var dto = Helper.RandomAssetDto();

            dto.TypeName = typeName;

            Assert.False(_dtoValidator.Validate(dto).IsValid);
        }

        [Theory]
        [MemberData(nameof(Helper.InvalidValues), MemberType = typeof(Helper))]
        public void AssetDto_Validation_Value_IsInvalid(decimal value)
        {
            var dto = Helper.RandomAssetDto();

            dto.Value = value;

            Assert.False(_dtoValidator.Validate(dto).IsValid);
        }
    }
}
