using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;
using FluentValidation;

using aiof.asset.data;

namespace aiof.asset.services
{
    public class AssetRepository : 
        BaseRepository<Asset, AssetDto>,
        IAssetRepository
    {
        private readonly AssetContext _context;

        private readonly AbstractValidator<AssetDto> _dtoValidator;

        public AssetRepository(
            ILogger<AssetRepository> logger,
            IMapper mapper,
            IEventRepository eventRepo,
            AssetContext context,
            AbstractValidator<AssetDto> dtoValidator,
            AbstractValidator<AssetSnapshotDto> snapshotDtoValidator)
            : base(logger, mapper, eventRepo, context, snapshotDtoValidator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dtoValidator = dtoValidator ?? throw new ArgumentNullException(nameof(dtoValidator));
        }

        public new async Task<IEnumerable<IAssetType>> GetTypesAsync()
        {
            return await base.GetTypesAsync();
        }

        public async Task<IAsset> GetAsync(
            int id,
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null)
        {
            return await base.GetAsync(id, snapshotsStartDate, snapshotsEndDate);
        }

        public async Task<IEnumerable<IAsset>> GetAsync(
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null,
            string type = null)
        {
            return await base.GetQuery(snapshotsStartDate, snapshotsEndDate, type)
                .ToListAsync();
        }

        public async Task<IAssetSnapshot> GetLatestSnapshotAsync(int assetId)
        {
            return await base.GetLatestSnapshotAsync(assetId);
        }

        public async Task<IAssetSnapshot> GetLatestSnapshotWithValueAsync(int assetId)
        {
            return await base.GetLatestSnapshotWithValueAsync(assetId);
        }

        public new async Task<IAsset> AddAsync(AssetDto dto)
        {
            await _dtoValidator.ValidateAndThrowAddAssetAsync(dto);

            return await base.AddAsync(dto);
        }

        public new async Task<IEnumerable<IAsset>> AddAsync(IEnumerable<AssetDto> dtos)
        {
            return await base.AddAsync(dtos);
        }

        public new async Task<IAssetSnapshot> AddSnapshotAsync(AssetSnapshotDto dto)
        {
            return await base.AddSnapshotAsync(dto);
        }

        public new async Task<IAsset> UpdateAsync(
            int id,
            AssetDto dto)
        {
            await _dtoValidator.ValidateAndThrowUpdateAssetAsync(dto);

            return await base.UpdateAsync(id, dto);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}
