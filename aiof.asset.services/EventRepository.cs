using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using AutoMapper;

using aiof.asset.data;

namespace aiof.asset.services
{
    public class EventRepository : IEventRepository
    {
        private readonly ILogger<EventRepository> _logger;
        private readonly IMapper _mapper;
        private readonly ITenant _tenant;

        public EventRepository(
            ILogger<EventRepository> logger,
            IMapper mapper,
            ITenant tenant)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        private async Task EmitAsync<T>(
            T assetEvent, 
            Asset asset)
            where T : AssetEvent
        {
            var eventEntity = _mapper.Map<EventEntity>(asset);
            var eventUser = _mapper.Map<EventUser>(_tenant);

            assetEvent.Entity = eventEntity;
            assetEvent.User = eventUser;

            var k = 1;
        }
    }
}
