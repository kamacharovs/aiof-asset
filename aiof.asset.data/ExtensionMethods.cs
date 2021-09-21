using System;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;
using AutoMapper;

namespace aiof.asset.data
{
    public static class ExtensionMethods
    {
        #region Validate Asset
        public static async Task<ValidationResult> ValidateAddAssetAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.AddRuleSet));
        }

        public static async Task<ValidationResult> ValidateUpdateAssetAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.UpdateRuleSet));
        }

        public static async Task ValidateAndThrowAddAssetAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetDto
        {
            await validator.ValidateAndThrowAddAsync(dto, Constants.AddRuleSet);
        }

        public static async Task ValidateAndThrowUpdateAssetAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetDto
        {
            await validator.ValidateAndThrowUpdateAsync(dto, Constants.UpdateRuleSet);
        }

        private static async Task ValidateAndThrowAddAsync<T>(
            this AbstractValidator<T> validator,
            T dto,
            string ruleSet)
        {
            var result = await validator.ValidateAsync(dto, o => o.IncludeRuleSets(ruleSet));

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        private static async Task ValidateAndThrowUpdateAsync<T>(
            this AbstractValidator<T> validator,
            T dto,
            string ruleSet)
        {
            var result = await validator.ValidateAsync(dto, o => o.IncludeRuleSets(ruleSet));

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
        #endregion

        #region Validate Stock
        public static async Task<ValidationResult> ValidateAddStockAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetStockDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.AddStockRuleSet));
        }

        public static async Task<ValidationResult> ValidateUpdateStockAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetStockDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.UpdateStockRuleSet));
        }

        public static async Task ValidateAndThrowAddStockAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetStockDto
        {
            await validator.ValidateAndThrowAddAsync(dto, Constants.AddStockRuleSet);
        }

        public static async Task ValidateAndThrowUpdateStockAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetStockDto
        {
            await validator.ValidateAndThrowUpdateAsync(dto, Constants.UpdateStockRuleSet);
        }
        #endregion

        #region Validate Home
        public static async Task<ValidationResult> ValidateAddHomeAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetHomeDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.AddHomeRuleSet));
        }

        public static async Task<ValidationResult> ValidateUpdateHomeAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetHomeDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.UpdateHomeRuleSet));
        }

        public static async Task ValidateAndThrowAddHomeAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetHomeDto
        {
            await validator.ValidateAndThrowAddAsync(dto, Constants.AddHomeRuleSet);
        }

        public static async Task ValidateAndThrowUpdateHomeAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetHomeDto
        {
            await validator.ValidateAndThrowUpdateAsync(dto, Constants.UpdateHomeRuleSet);
        }
        #endregion

        #region Validate Snapshot
        public static async Task<ValidationResult> ValidateAddSnapshotAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetSnapshotDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.AddSnapshotRuleSet));
        }

        public static async Task<ValidationResult> ValidateUpdateSnapshotAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetSnapshotDto
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.UpdateSnapshotRuleSet));
        }

        public static async Task ValidateAndThrowAddSnapshotAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetSnapshotDto
        {
            await validator.ValidateAndThrowAddAsync(dto, Constants.AddSnapshotRuleSet);
        }

        public static async Task ValidateAndThrowUpdateSnapshotAsync<T>(this AbstractValidator<T> validator, T dto)
            where T : AssetSnapshotDto
        {
            await validator.ValidateAndThrowUpdateAsync(dto, Constants.UpdateSnapshotRuleSet);
        }
        #endregion     

        #region Automapper
        public static T MergeInto<T>(this IMapper mapper, params object[] objects)
        {
            var first = mapper.Map<T>(objects.First());

            return objects
                .Skip(1)
                .Aggregate(first, (r, obj) => mapper.Map(obj, r));
        }
        #endregion

        #region Generic
        public static bool IsValid(this AssetSnapshotDto dto)
        {
            return dto.Name is not null
                || dto.TypeName is not null
                || dto.Value.HasValue;
        }
        #endregion
    }
}