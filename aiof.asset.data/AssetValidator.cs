using FluentValidation;

namespace aiof.asset.data
{
    public static class CommonValidator
    {
        public const decimal MinimumValue = 0M;
        public const decimal MaximumValue = 99999999M;

        public static string ValueMessage = $"Value must be between {MinimumValue} and {MaximumValue}";
    }

    public class AssetDtoValidator : AbstractValidator<AssetDto>
    {
        public AssetDtoValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.TypeName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThan(CommonValidator.MaximumValue)
                .WithMessage(CommonValidator.ValueMessage);
        }
    }

    public class AssetSnapshotDtoValidator : AbstractValidator<AssetSnapshotDto>
    {
        public AssetSnapshotDtoValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.AssetId)
                .NotNull()
                .NotEqual(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .When(x => x.Name != null);

            RuleFor(x => x.TypeName)
                .NotEmpty()
                .When(x => x.Name != null);

            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(CommonValidator.MinimumValue)
                .LessThan(CommonValidator.MaximumValue)
                .WithMessage(CommonValidator.ValueMessage)
                .When(x => x.Value.HasValue);
        }
    }
}
