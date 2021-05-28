using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using AutoMapper;
using RestSharp;

using aiof.asset.data;

namespace aiof.asset.services
{
    public class EventRepository : IEventRepository
    {
        private readonly ILogger<EventRepository> _logger;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ITenant _tenant;
        private readonly IRestClient _client;

        public EventRepository(
            ILogger<EventRepository> logger,
            IConfiguration config,
            IMapper mapper,
            ITenant tenant,
            IRestClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void Emit<T>(Asset asset)
            where T : AssetEvent, new()
        {
            var eventEntity = _mapper.Map<EventEntity>(asset);
            var eventUser = _mapper.Map<EventUser>(_tenant);

            var assetEvent = new T
            {
                Entity = eventEntity,
                User = eventUser
            };

            try
            {
                var request = new RestRequest("/emit", Method.POST).AddJsonBody(assetEvent);
                var result = _client.Post<object>(request);
            }
            catch (Exception e)
            {
                _logger.LogError("{Tenant} | Error while emitting event={EventName}. Message={EventingErrorMessage}",
                    _tenant.Log,
                    nameof(T),
                    e.Message);
            }
        }
    }
}
