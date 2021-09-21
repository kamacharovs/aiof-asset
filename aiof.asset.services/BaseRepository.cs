using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;
using FluentValidation;

using aiof.asset.data;

namespace aiof.asset.services
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseRepository<T, TDto>
        where T : Asset
        where TDto : AssetDto
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepo;
        private readonly AssetContext _context;
        private readonly AbstractValidator<AssetSnapshotDto> _snapshotDtoValidator;

        public BaseRepository(
            ILogger logger,
            IMapper mapper,
            IEventRepository eventRepo,
            AssetContext context,
            AbstractValidator<AssetSnapshotDto> snapshotDtoValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventRepo = eventRepo ?? throw new ArgumentNullException(nameof(eventRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _snapshotDtoValidator = snapshotDtoValidator ?? throw new ArgumentNullException(nameof(snapshotDtoValidator));
        }

        public IQueryable<AssetType> GetTypesQuery(bool asNoTracking = true)
        {
            var query = _context.AssetTypes
                .AsQueryable();

            return asNoTracking
                ? query.AsNoTracking()
                : query;
        }

        public IQueryable<AssetSnapshot> GetSnapshotQuery(bool asNoTracking = true)
        {
            var assetSnapshotsQuery = _context.AssetSnapshots;

            return asNoTracking
                ? assetSnapshotsQuery.AsNoTracking()
                : assetSnapshotsQuery;
        }

        public IQueryable<T> GetBaseQuery(bool asNoTracking = true)
        {
            var assetsQuery = _context.Set<T>();

            return asNoTracking
                ? assetsQuery.AsNoTracking()
                : assetsQuery;
        }

        public IQueryable<T> GetQuery(
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null,
            string type = null,
            bool asNoTracking = true)
        {
            snapshotsStartDate = snapshotsStartDate ?? DateTime.UtcNow.AddMonths(-6);
            snapshotsEndDate = snapshotsEndDate ?? DateTime.UtcNow;

            if (snapshotsEndDate < snapshotsStartDate)
                throw new AssetFriendlyException(HttpStatusCode.BadRequest,
                    $"Snapshots end date cannot be earlier than start date");

            var dbSet = type is null
                ? _context.Set<T>().AsQueryable()
                : _context.Set<T>().Where(x => x.TypeName == type);

            var assetsQuery = dbSet
                .Include(x => x.Type)
                .Include(x => x.Snapshots
                    .Where(x => x.Created >= snapshotsStartDate && x.Created <= snapshotsEndDate)
                    .OrderByDescending(x => x.Created));

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

        public async Task<IAssetSnapshot> GetLatestSnapshotAsync(
            int assetId,
            bool asNoTracking = true)
        {
            return await GetSnapshotQuery(asNoTracking)
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();
        }

        public async Task<IAssetSnapshot> GetLatestSnapshotWithValueAsync(
            int assetId,
            bool asNoTracking = true)
        {
            return await GetSnapshotQuery(asNoTracking)
                .Where(x => x.AssetId == assetId
                    && x.Value.HasValue)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();
        }

        public async Task<IAsset> GetAsync(
            int id,
            DateTime? snapshotsStartDate = null,
            DateTime? snapshotsEndDate = null,
            bool asNoTracking = true)
        {
            var asset = await GetQuery(snapshotsStartDate, snapshotsEndDate, null, asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AssetNotFoundException($"Asset with Id={id} was not found");

            if (asset.Snapshots.Count() == 0)
                asset.Snapshots.Add(await GetLatestSnapshotAsync(asset.Id) as AssetSnapshot);

            return asset;
        }

        public async Task<IAsset> AddAsync(TDto dto)
        {
            var asset = _mapper.Map<T>(dto);

            asset.UserId = _context.Tenant.UserId;

            await _context.Set<T>().AddAsync(asset);
            await _context.SaveChangesAsync();

            // Create snapshot entry
            var snapshotDto = _mapper.Map<AssetSnapshotDto>(dto);

            if (snapshotDto.IsValid())
            {
                snapshotDto.AssetId = asset.Id;

                await AddSnapshotAsync(snapshotDto);
            }

            _logger.LogInformation("{Tenant} | Created Asset={AssetType} with Id={AssetId}, PublicKey={AssetPublicKey} and UserId={AssetUserId}",
                _context.Tenant.Log,
                typeof(T).Name,
                asset.Id,
                asset.PublicKey,
                asset.UserId);

            // Emit event
            await _eventRepo.EmitAsync<AssetAddedEvent>(asset);

            return asset;
        }

        public async Task<IEnumerable<IAsset>> AddAsync(IEnumerable<TDto> dtos)
        {
            if (dtos.Count() > 10)
                throw new AssetFriendlyException(HttpStatusCode.BadRequest,
                    $"Cannot add more than 10 Assets at a time");

            var assetsAdded = new List<IAsset>();

            foreach (var dto in dtos)
            {
                try
                {
                    assetsAdded.Add(await AddAsync(dto));
                }
                catch (ValidationException)
                {
                    continue;
                }
            }

            _logger.LogInformation("{Tenant} | Added {TotalAssets} Assets from total of {TotalDtos} requested",
                _context.Tenant.Log,
                assetsAdded.Count(),
                dtos.Count());

            return assetsAdded.AsEnumerable();
        }

        public async Task<IAssetSnapshot> AddSnapshotAsync(AssetSnapshotDto dto)
        {
            await _snapshotDtoValidator.ValidateAndThrowAddSnapshotAsync(dto);

            var snapshot = _mapper.Map<AssetSnapshot>(dto);

            // Calculate ValueChange
            if (dto.Value.HasValue)
            {
                var latestSnapshot = await GetLatestSnapshotWithValueAsync(dto.AssetId);
                var latestSnapshotValue = latestSnapshot is not null
                    ? latestSnapshot.Value
                    : snapshot.ValueChange;

                snapshot.ValueChange = latestSnapshotValue == 0
                    ? snapshot.ValueChange
                    : dto.Value - latestSnapshotValue;
            }

            await _context.AssetSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();

            _logger.LogInformation("{Tenant} | Created AssetSnapshot for AssetId={AssetId}",
                _context.Tenant.Log,
                snapshot.AssetId);

            return snapshot;
        }

        public async Task<IAsset> UpdateAsync(
            int id,
            TDto dto)
        {
            var asset = await GetAsync(id, asNoTracking: false) as T
                ?? throw new AssetFriendlyException(HttpStatusCode.BadRequest,
                    $"Asset is not of type {Constants.ClassToTypeMap[typeof(T).Name]}");

            asset = _mapper.Map(dto, asset);

            _context.Set<T>().Update(asset);
            await _context.SaveChangesAsync();

            // Create snapshot entry
            var snapshotDto = _mapper.Map<AssetSnapshotDto>(dto);
       
            if (snapshotDto.IsValid())
            {
                snapshotDto.AssetId = asset.Id;

                await AddSnapshotAsync(snapshotDto);
            }

            _logger.LogInformation("{Tenant} | Updated Asset={AssetType} with Id={AssetId}, PublicKey={AssetPublicKey} and UserId={AssetUserId}",
                _context.Tenant.Log,
                typeof(T).Name,
                asset.Id,
                asset.PublicKey,
                asset.UserId);

            // Emit event
            await _eventRepo.EmitAsync<AssetUpdatedEvent>(asset);

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

            // Emit event
            await _eventRepo.EmitAsync<AssetDeletedEvent>(asset);
        }
    }
}
