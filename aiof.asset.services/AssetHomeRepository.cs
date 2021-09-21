using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using AutoMapper;
using FluentValidation;

using aiof.asset.data;

namespace aiof.asset.services
{
    public class AssetHomeRepository :
        BaseRepository<AssetHome, AssetHomeDto>,
        IAssetHomeRepository
    {
        private readonly AbstractValidator<AssetHomeDto> _dtoValidator;

        public AssetHomeRepository(
            ILogger<AssetHomeRepository> logger,
            IMapper mapper,
            IEventRepository eventRepo,
            AssetContext context,
            AbstractValidator<AssetHomeDto> dtoValidator,
            AbstractValidator<AssetSnapshotDto> snapshotDtoValidator)
            : base(logger, mapper, eventRepo, context, snapshotDtoValidator)
        {
            _dtoValidator = dtoValidator ?? throw new ArgumentNullException(nameof(dtoValidator));
        }

        public new async Task<IAsset> AddAsync(AssetHomeDto dto)
        {
            await _dtoValidator.ValidateAndThrowAddHomeAsync(dto);

            dto.TypeName = AssetTypes.Home;

            return await base.AddAsync(dto);
        }

        public new async Task<IAsset> UpdateAsync(int id, AssetHomeDto dto)
        {
            await _dtoValidator.ValidateAndThrowUpdateHomeAsync(dto);

            return await base.UpdateAsync(id, dto);
        }
    }
}
