using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace aiof.asset.data
{
    public class AssetProblemDetail : IAssetProblemDetail
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }

        [JsonPropertyName("errors")]
        public IEnumerable<string> Errors { get; set; }
    }

    public class AssetProblemDetailBase : IAssetProblemDetailBase
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
