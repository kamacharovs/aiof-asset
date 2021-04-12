using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace aiof.asset.data
{
    public interface IAssetProblemDetail
    {
        [JsonPropertyName("code")]
        int? Code { get; set; }

        [JsonPropertyName("message")]
        string Message { get; set; }

        [JsonPropertyName("traceId")]
        string TraceId { get; set; }

        [JsonPropertyName("errors")]
        IEnumerable<string> Errors { get; set; }
    }

    public interface IAssetProblemDetailBase
    {
        [JsonPropertyName("code")]
        int? Code { get; set; }

        [JsonPropertyName("message")]
        string Message { get; set; }
    }
}
