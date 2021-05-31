using System;

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

            RuleFor(assetType => assetType)
                .NotNull()
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

            RuleSet(Constants.AddRuleSet, () =>
            {
                RuleFor(x => x)
                    .NotNull();

                RuleFor(x => x.Name)
                    .NotNull()
                    .NotEmpty()
                    .MaximumLength(100);

                RuleFor(x => x.TypeName)
                    .SetValidator(new AssetTypeValidator(_context))
                    .When(x => x.TypeName != null);

                RuleFor(x => x.Value)
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThan(CommonValidator.MaximumValue)
                    .WithMessage(CommonValidator.ValueMessage);
            });

            RuleSet(Constants.UpdateRuleSet, () =>
            {
                RuleFor(x => x)
                    .NotNull();

                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(100)
                    .When(x => x.Name != null);

                RuleFor(x => x.TypeName)
                    .SetValidator(new AssetTypeValidator(_context))
                    .When(x => x.TypeName != null);

                RuleFor(x => x.Value)
                    .NotNull()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThan(CommonValidator.MaximumValue)
                    .WithMessage(CommonValidator.ValueMessage)
                    .When(x => x.Value.HasValue);
            });
        }
    }

    public class AssetStockDtoValidator : AbstractValidator<AssetStockDto>
    {
        private readonly AssetContext _context;

        public AssetStockDtoValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleSet(Constants.AddStockRuleSet, () =>
            {
                RuleFor(x => x)
                    .NotNull()
                    .SetValidator(new AssetDtoValidator(_context));

                RuleFor(x => x.TickerSymbol)
                    .NotNull()
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
                RuleFor(x => x)
                    .NotNull()
                    .SetValidator(new AssetDtoValidator(_context));

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

    public class AssetSnapshotDtoValidator : AbstractValidator<AssetSnapshotDto>
    {
        private readonly AssetContext _context;

        public AssetSnapshotDtoValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleSet(Constants.AddSnapshotRuleSet, () =>
            {
                RuleFor(x => x)
                    .NotNull();

                RuleFor(x => x.AssetId)
                    .NotNull()
                    .NotEmpty()
                    .NotEqual(0);

                RuleFor(x => x.Name)
                    .NotNull()
                    .NotEmpty()
                    .MaximumLength(100);

                RuleFor(x => x.TypeName)
                    .SetValidator(new AssetTypeValidator(_context));

                RuleFor(x => x.Value)
                    .NotNull()
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThan(CommonValidator.MaximumValue)
                    .WithMessage(CommonValidator.ValueMessage);
            });

            RuleSet(Constants.UpdateSnapshotRuleSet, () =>
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
                    .SetValidator(new AssetTypeValidator(_context))
                    .When(x => x.TypeName != null);

                RuleFor(x => x.Value)
                    .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                    .LessThan(CommonValidator.MaximumValue)
                    .WithMessage(CommonValidator.ValueMessage)
                    .When(x => x.Value.HasValue);
            });
        }
    }
}
