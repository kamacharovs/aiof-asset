using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;
using FluentValidation;

using aiof.asset.data;

namespace aiof.asset.services
{
    public class AssetRepository : IAssetRepository
    {
        private readonly ILogger<AssetRepository> _logger;
        private readonly IMapper _mapper;
        private readonly AssetContext _context;
        private readonly AbstractValidator<AssetDto> _dtoValidator;
        private readonly AbstractValidator<AssetSnapshotDto> _snapshotDtoValidator;

        public AssetRepository(
            ILogger<AssetRepository> logger,
            IMapper mapper,
            AssetContext context,
            AbstractValidator<AssetDto> dtoValidator,
            AbstractValidator<AssetSnapshotDto> snapshotDtoValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dtoValidator = dtoValidator ?? throw new ArgumentNullException(nameof(dtoValidator));
            _snapshotDtoValidator = snapshotDtoValidator ?? throw new ArgumentNullException(nameof(snapshotDtoValidator));
        }

        private IQueryable<AssetType> GetTypesQuery(bool asNoTracking = true)
        {
            var query = _context.AssetTypes
                .AsQueryable();

            return asNoTracking
                ? query.AsNoTracking()
                : query;
        }

        private IQueryable<Asset> GetBaseQuery(bool asNoTracking = true)
        {
            var assetsQuery = _context.Assets;

            return asNoTracking
                ? assetsQuery.AsNoTracking()
                : assetsQuery;
        }

        private IQueryable<AssetSnapshot> GetSnapshotQuery(bool asNoTracking = true)
        {
            var assetSnapshotsQuery = _context.AssetSnapshots;

            return asNoTracking
                ? assetSnapshotsQuery.AsNoTracking()
                : assetSnapshotsQuery;
        }

        private IQueryable<Asset> GetQuery(
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null,
            bool asNoTracking = true)
        {
            snapshotsStartDate = snapshotsStartDate ?? DateTime.UtcNow.AddMonths(-6);
            snapshotsEndDate = snapshotsEndDate ?? DateTime.UtcNow;

            var assetsQuery = _context.Assets
                .Include(x => x.Type)
                .Include(x => x.Snapshots
                    .Where(x => x.Created > snapshotsStartDate && x.Created <= snapshotsEndDate)
                    .OrderByDescending(x => x.Created))
                .AsQueryable();

            return asNoTracking
                ? assetsQuery.AsNoTracking()
                : assetsQuery;
        }

        public async Task<IEnumerable<IAssetType>> GetTypesAsync()
        {
            return await GetTypesQuery()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<IAsset> GetAsync(
            int id,
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null,
            bool asNoTracking = true)
        {
            return await GetQuery(snapshotsStartDate, snapshotsEndDate, asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AssetNotFoundException($"Asset with Id={id} was not found");
        }

        public async Task<IEnumerable<IAsset>> GetAsync(
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null)
        {
            return await GetQuery(snapshotsStartDate, snapshotsEndDate)
                .ToListAsync();
        }

        public async Task<IAssetSnapshot> GetLatestSnapshotAsync(
            int assetId,
            bool asNoTracking = true)
        {
            return await GetSnapshotQuery(asNoTracking)
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();
        }

        public async Task<IAsset> AddAsync(AssetDto dto)
        {
            await _dtoValidator.ValidateAndThrowAsync(dto);

            var asset = _mapper.Map<Asset>(dto);

            asset.UserId = _context.Tenant.UserId;

            await _context.Assets.AddAsync(asset);
            await _context.SaveChangesAsync();

            // Create snapshot entry
            var snapshotDto = _mapper.Map<AssetSnapshotDto>(dto);

            snapshotDto.AssetId = asset.Id;

            await AddSnapshotAsync(snapshotDto);

            _logger.LogInformation("{Tenant} | Created Asset with Id={AssetId}, PublicKey={AssetPublicKey} and UserId={AssetUserId}",
                _context.Tenant.Log,
                asset.Id,
                asset.PublicKey,
                asset.UserId);

            return asset;
        }

        public async Task<IAssetSnapshot> AddSnapshotAsync(AssetSnapshotDto dto)
        {
            await _snapshotDtoValidator.ValidateAndThrowAsync(dto);

            var snapshot = _mapper.Map<AssetSnapshot>(dto);

            await _context.AssetSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();

            _logger.LogInformation("{Tenant} | Created AssetSnapshot for AssetId={AssetId}",
                _context.Tenant.Log,
                snapshot.AssetId);

            return snapshot;
        }

        public async Task<IAsset> UpdateAsync(
            int id,
            AssetDto dto)
        {
            await _dtoValidator.ValidateAndThrowAsync(dto);

            var asset = await GetAsync(id, asNoTracking: false) as Asset;

            asset = _mapper.Map(dto, asset);

            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();

            // Create snapshot entry
            var snapshotDto = _mapper.Map<AssetSnapshotDto>(dto);

            snapshotDto.AssetId = asset.Id;

            await AddSnapshotAsync(snapshotDto);

            _logger.LogInformation("{Tenant} | Updated Asset with Id={AssetId}, PublicKey={AssetPublicKey} and UserId={AssetUserId}",
                _context.Tenant.Log,
                asset.Id,
                asset.PublicKey,
                asset.UserId);

            return asset;
        }

        public async Task DeleteAsync(int id)
        {
            var asset = await GetBaseQuery(false)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AssetNotFoundException($"Asset with Id={id} was not found");

            asset.IsDeleted = true;

            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();

            _logger.LogInformation("{Tenant} | Soft Deleted Asset with Id={AssetId}",
                _context.Tenant.Log,
                id);
        }
    }
}
