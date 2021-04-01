using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;

using aiof.asset.data;

namespace aiof.asset.services
{
    public class AssetRepository : IAssetRepository
    {
        private readonly ILogger<AssetRepository> _logger;
        private readonly IMapper _mapper;
        private readonly AssetContext _context;

        public AssetRepository(
            ILogger<AssetRepository> logger,
            IMapper mapper,
            AssetContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IQueryable<Asset> GetBaseQuery(bool asNoTracking = true)
        {
            var assetsQuery = _context.Assets
                .Include(x => x.Type);

            return asNoTracking
                ? assetsQuery.AsNoTracking()
                : assetsQuery;
        }

        private IQueryable<Asset> GetQuery(
            DateTime? snapshotStartDate = null,
            DateTime? snapshotEndDate = null,
            bool asNoTracking = true)
        {
            snapshotStartDate = snapshotStartDate ?? DateTime.UtcNow.AddMonths(-6);
            snapshotEndDate = snapshotEndDate ?? DateTime.UtcNow;

            var assetsQuery = _context.Assets
                .Include(x => x.Type)
                .Include(x => x.Snapshots
                    .Where(x => x.Created > snapshotStartDate && x.Created <= snapshotEndDate)
                    .OrderByDescending(x => x.Created))
                .AsQueryable();

            return asNoTracking
                ? assetsQuery.AsNoTracking()
                : assetsQuery;
        }

        private IQueryable<AssetType> GetTypesQuery(bool asNoTracking = true)
        {
            var query = _context.AssetTypes
                .AsQueryable();

            return asNoTracking
                ? query.AsNoTracking()
                : query;
        }

        public async Task<IAsset> GetAsync(
            int id,
            bool asNoTracking = true)
        {
            return await GetBaseQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AssetNotFoundException($"Asset with Id={id} was not found");
        }

        public async Task<IAsset> GetWithSnapshotsAsync(
            int id,
            DateTime? snapshotStartDate = null,
            DateTime? snapshotEndDate = null,
            bool asNoTracking = true)
        {
            return await GetQuery(snapshotStartDate, snapshotEndDate, asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AssetNotFoundException($"Asset with Id={id} was not found");
        }

    }
}
