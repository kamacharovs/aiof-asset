using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using FluentValidation;

namespace aiof.asset.data
{
    public static class CommonValidator
    {
        public const decimal MinimumValue = 0M;
        public const decimal MaximumValue = 99999999M;
        public const decimal MinimumPercentValue = 0M;
        public const decimal MaximumPercentValue = 100M;

        public static string ValueMessage = $"Value must be between {MinimumValue} and {MaximumValue}";
        public static string PercentValueMessage = $"Percent must be between {MinimumPercentValue} and {MaximumPercentValue}";
        public static string TypeNameMessage = $"Invalid {nameof(AssetDto.TypeName)}";

        public static string AssetUpdateMessage = $"You must specify at least one field to update. '{nameof(AssetDto.Name)}', '{nameof(AssetDto.TypeName)}' or '{nameof(AssetDto.Value)}'";

        public static bool BeValidPercent(double? value)
        {
            var valuePerc = Math.Round((double)(value * 100), 4);

            return value.HasValue
                ? valuePerc > 0 && valuePerc <= 100
                : false;
        }
    }

    public class AssetTypeValidator : AbstractValidator<string>
    {
        private readonly AssetContext _context;

        public AssetTypeValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleSet(Constants.AddRuleSet, () => { SetTypeRules(); });
            RuleSet(Constants.UpdateRuleSet, () => { SetTypeRules(); });
            RuleSet(Constants.AddStockRuleSet, () => { SetTypeRules(); });
            RuleSet(Constants.UpdateStockRuleSet, () => { SetTypeRules(); });
            RuleSet(Constants.AddHomeRuleSet, () => { SetTypeRules(); });
            RuleSet(Constants.UpdateHomeRuleSet, () => { SetTypeRules(); });
            RuleSet(Constants.AddSnapshotRuleSet, () => { SetTypeRules(); });
            RuleSet(Constants.UpdateSnapshotRuleSet, () => { SetTypeRules(); });

            SetTypeRules();
        }

        public void SetTypeRules()
        {
            RuleFor(assetType => assetType)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(async (type, cancellation) =>
                {
                    return await _context.AssetTypes
                        .AnyAsync(x => x.Name == type);
                })
                .WithMessage(CommonValidator.TypeNameMessage);
        }
    }

    public class AssetDtoValidator : AbstractValidator<AssetDto>
    {
        private readonly AssetContext _context;

        public AssetDtoValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleSet(Constants.AddRuleSet, () => { SetAddRules(); });
            RuleSet(Constants.AddStockRuleSet, () => { SetAddRules(); });
            RuleSet(Constants.AddHomeRuleSet, () => { SetAddRules(); });

            RuleSet(Constants.UpdateRuleSet, () => { SetUpdateRules(); });
            RuleSet(Constants.UpdateStockRuleSet, () => { SetUpdateRules(); });
            RuleSet(Constants.UpdateHomeRuleSet, () => { SetUpdateRules(); });
        }

        public void SetAddRules()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.TypeName)
                .NotEmpty()
                .SetValidator(new AssetTypeValidator(_context));

            RuleFor(x => x.Value)
                .NotNull()
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThan(CommonValidator.MaximumValue)
                .WithMessage(CommonValidator.ValueMessage);
        }

        public void SetUpdateRules()
        {
            RuleFor(x => x)
                .Must(x =>
                {
                    var type = x.GetType().Name;
                    var areAllNull =
                        x.Name is null
                        && x.TypeName is null
                        && !x.Value.HasValue
                        && type != nameof(AssetStockDto);

                    return !areAllNull;
                })
                .WithMessage(CommonValidator.AssetUpdateMessage);

            RuleFor(x => x.Name)
                .MaximumLength(100)
                .When(x => x.Name != null);

            RuleFor(x => x.TypeName)
                .NotEmpty()
                .SetValidator(new AssetTypeValidator(_context))
                .When(x => x.TypeName != null);

            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThan(CommonValidator.MaximumValue)
                .WithMessage(CommonValidator.ValueMessage)
                .When(x => x.Value.HasValue);
        }
    }

    public class AssetStockDtoValidator : AbstractValidator<AssetStockDto>
    {
        private readonly AssetContext _context;

        public AssetStockDtoValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

            Include(new AssetDtoValidator(_context));

            RuleSet(Constants.AddStockRuleSet, () =>
            {
                RuleFor(x => x.TickerSymbol)
                    .NotEmpty()
                    .MaximumLength(50);

                RuleFor(x => x.Shares)
                    .GreaterThan(0)
                    .When(x => x.Shares.HasValue);

                RuleFor(x => x.ExpenseRatio)
                    .GreaterThan(0)
                    .Must(x =>
                    {
                        return CommonValidator.BeValidPercent(x);
                    })
                    .WithMessage(CommonValidator.PercentValueMessage)
                    .When(x => x.ExpenseRatio.HasValue);

                RuleFor(x => x.DividendYield)
                    .GreaterThan(0)
                    .Must(x =>
                    {
                        return CommonValidator.BeValidPercent(x);
                    })
                    .WithMessage(CommonValidator.PercentValueMessage)
                    .When(x => x.DividendYield.HasValue);
            });

            RuleSet(Constants.UpdateStockRuleSet, () =>
            {
                RuleFor(x => x.TickerSymbol)
                    .NotEmpty()
                    .MaximumLength(50)
                    .When(x => x.TickerSymbol != null);

                RuleFor(x => x.Shares)
                    .GreaterThan(0)
                    .When(x => x.Shares.HasValue);

                RuleFor(x => x.ExpenseRatio)
                    .GreaterThan(0)
                    .Must(x =>
                    {
                        return CommonValidator.BeValidPercent(x);
                    })
                    .WithMessage(CommonValidator.PercentValueMessage)
                    .When(x => x.ExpenseRatio.HasValue);

                RuleFor(x => x.DividendYield)
                    .GreaterThan(0)
                    .Must(x =>
                    {
                        return CommonValidator.BeValidPercent(x);
                    })
                    .WithMessage(CommonValidator.PercentValueMessage)
                    .When(x => x.DividendYield.HasValue);
            });
        }
    }

    public class AssetHomeDtoValidator : AbstractValidator<AssetHomeDto>
    {
        private readonly AssetContext _context;

        public AssetHomeDtoValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

            Include(new AssetDtoValidator(_context));

            RuleSet(Constants.AddStockRuleSet, () =>
            {
                RuleFor(x => x.HomeType)
                    .NotEmpty()
                    .MaximumLength(100);

                RuleFor(x => x.LoanValue)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumValue);

                RuleFor(x => x.MonthlyMortgage)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumValue);

                RuleFor(x => x.MortgageRate)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumPercentValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumPercentValue);

                RuleFor(x => x.DownPayment)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumValue);

                SetCommonRules();
            });

            RuleSet(Constants.UpdateStockRuleSet, () =>
            {
                RuleFor(x => x.HomeType)
                    .NotEmpty()
                    .MaximumLength(100)
                    .When(x => x.HomeType != null);

                RuleFor(x => x.LoanValue)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumValue)
                    .When(x => x.LoanValue.HasValue);

                RuleFor(x => x.MonthlyMortgage)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumValue)
                    .When(x => x.MonthlyMortgage.HasValue);

                RuleFor(x => x.MortgageRate)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumPercentValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumPercentValue)
                    .When(x => x.MortgageRate.HasValue);

                RuleFor(x => x.DownPayment)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThanOrEqualTo(CommonValidator.MaximumValue)
                    .When(x => x.DownPayment.HasValue);

                SetCommonRules();
            });
        }

        public void SetCommonRules()
        {
            RuleFor(x => x.AnnualInsurance)
                .NotEmpty()
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThanOrEqualTo(CommonValidator.MaximumValue)
                .When(x => x.AnnualInsurance.HasValue);

            RuleFor(x => x.AnnualPropertyTax)
                .NotEmpty()
                .GreaterThanOrEqualTo(CommonValidator.MinimumPercentValue)
                .LessThanOrEqualTo(CommonValidator.MaximumPercentValue)
                .When(x => x.AnnualPropertyTax.HasValue);

            RuleFor(x => x.ClosingCosts)
                .NotEmpty()
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThanOrEqualTo(CommonValidator.MaximumValue)
                .When(x => x.ClosingCosts.HasValue);

            RuleFor(x => x.IsRefinanced)
                .NotEmpty()
                .When(x => x.IsRefinanced.HasValue);
        }
    }

    public class AssetSnapshotDtoValidator : AbstractValidator<AssetSnapshotDto>
    {
        private readonly AssetContext _context;

        public AssetSnapshotDtoValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleSet(Constants.AddSnapshotRuleSet, () => { SetupSnapshotRules(); });
            RuleSet(Constants.UpdateSnapshotRuleSet, () => { SetupSnapshotRules(); });
        }

        public void SetupSnapshotRules()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.AssetId)
                .NotNull()
                .NotEqual(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .When(x => x.Name != null);

            RuleFor(x => x.TypeName)
                .NotEmpty()
                .SetValidator(new AssetTypeValidator(_context))
                .When(x => x.TypeName != null);

            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThan(CommonValidator.MaximumValue)
                .WithMessage(CommonValidator.ValueMessage)
                .When(x => x.Value.HasValue);
        }
    }
}
