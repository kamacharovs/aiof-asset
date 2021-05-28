using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using AutoMapper;
using RestSharp;
using Polly;

using aiof.asset.data;
using Microsoft.AspNetCore.Http;

namespace aiof.asset.services
{
    public class EventRepository : IEventRepository
    {
        private readonly ILogger<EventRepository> _logger;
        private readonly IMapper _mapper;
        private readonly ITenant _tenant;
        private readonly IRestClient _client;

        public EventRepository(
            ILogger<EventRepository> logger,
            IMapper mapper,
            ITenant tenant,
            IRestClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task EmitAsync<T>(Asset asset)
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
                await Policy.Handle<HttpRequestException>()
                    .OrResult<IRestResponse>(x => (int)x.StatusCode == StatusCodes.Status500InternalServerError)
                    .RetryAsync(3)
                    .ExecuteAsync(async () =>
                    {
                        var request = new RestRequest("/emit", Method.POST).AddJsonBody(assetEvent);

                        return await _client.ExecuteAsync(request);
                    });
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
