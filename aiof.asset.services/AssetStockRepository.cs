using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using AutoMapper;
using FluentValidation;

using aiof.asset.data;

namespace aiof.asset.services
{
    public class AssetStockRepository :
        BaseRepository<AssetStock, AssetStockDto>,
        IAssetStockRepository
    {
        private readonly AbstractValidator<AssetStockDto> _dtoValidator;

        public AssetStockRepository(
            ILogger<AssetStockRepository> logger,
            IMapper mapper,
            IEventRepository eventRepo,
            AssetContext context,
            AbstractValidator<AssetStockDto> dtoValidator,
            AbstractValidator<AssetSnapshotDto> snapshotDtoValidator)
            : base(logger, mapper, eventRepo, context, snapshotDtoValidator)
        {
            _dtoValidator = dtoValidator ?? throw new ArgumentNullException(nameof(dtoValidator));
        }

        public new async Task<IAsset> AddAsync(AssetStockDto dto)
        {
            await _dtoValidator.ValidateAndThrowAddStockAsync(dto);

            return await base.AddAsync(dto);
        }

        public new async Task<IAsset> UpdateAsync(int id, AssetStockDto dto)
        {
            await _dtoValidator.ValidateAndThrowUpdateStockAsync(dto);

            return await base.UpdateAsync(id, dto);
        }
    }
}
