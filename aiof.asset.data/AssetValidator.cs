using System;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using FluentValidation;

namespace aiof.asset.data
{
    public static class CommonValidator
    {
        public const decimal MinimumValue = 0M;
        public const decimal MaximumValue = 99999999M;

        public static string ValueMessage = $"Value must be between {MinimumValue} and {MaximumValue}";
        public static string TypeNameMessage = $"Invalid {nameof(AssetDto.TypeName)}";
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
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThan(CommonValidator.MaximumValue)
                .WithMessage(CommonValidator.ValueMessage)
                .When(x => x.Value.HasValue);
        }
    }

    public class AssetSnapshotDtoValidator : AbstractValidator<AssetSnapshotDto>
    {
        private readonly AssetContext _context;

        public AssetSnapshotDtoValidator(AssetContext context)
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            _context = context ?? throw new ArgumentNullException(nameof(context));

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
        }
    }
}
