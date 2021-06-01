using System;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

namespace aiof.asset.data
{
    public static class ExtensionMethods
    {
        #region Validate adAsset
        public static async Task<ValidationResult> ValidateAddAssetAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.AddRuleSet));
        }
        public static async Task<ValidationResult> ValidateUpdateAssetAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.UpdateRuleSet));
        }
        public static async Task ValidateAndThrowAddAssetAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            await validator.ValidateAndThrowAddAsync(dto, Constants.AddRuleSet);
        }
        public static async Task ValidateAndThrowUpdateAssetAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            await validator.ValidateAndThrowUpdateAsync(dto, Constants.UpdateRuleSet);
        }
        #endregion

        #region Validate Stock
        public static async Task<ValidationResult> ValidateAddStockAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.AddStockRuleSet));
        }
        public static async Task<ValidationResult> ValidateUpdateStockAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.UpdateStockRuleSet));
        }
        public static async Task ValidateAndThrowAddStockAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            await validator.ValidateAndThrowAddAsync(dto, Constants.AddStockRuleSet);
        }
        public static async Task ValidateAndThrowUpdateStockAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            await validator.ValidateAndThrowUpdateAsync(dto, Constants.UpdateStockRuleSet);
        }
        #endregion

        #region Validate Snapshot
        public static async Task<ValidationResult> ValidateAddSnapshotAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.AddSnapshotRuleSet));
        }
        public static async Task<ValidationResult> ValidateUpdateSnapshotAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            return await validator.ValidateAsync(dto, o => o.IncludeRuleSets(Constants.UpdateSnapshotRuleSet));
        }
        public static async Task ValidateAndThrowAddSnapshotAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            await validator.ValidateAndThrowAddAsync(dto, Constants.AddSnapshotRuleSet);
        }
        public static async Task ValidateAndThrowUpdateSnapshotAsync<T>(
            this IValidator<T> validator, 
            T dto)
        {
            await validator.ValidateAndThrowUpdateAsync(dto, Constants.UpdateSnapshotRuleSet);
        }
        #endregion

        private static async Task ValidateAndThrowAddAsync<T>(
            this IValidator<T> validator, 
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
            this IValidator<T> validator, 
            T dto,
            string ruleSet)
        {
            var result = await validator.ValidateAsync(dto, o => o.IncludeRuleSets(ruleSet));

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}