using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace aiof.asset.data
{
    public interface ITenant
    {
        [JsonPropertyName("user_id")]
        int UserId { get; set; }

        [JsonPropertyName("client_id")]
        int ClientId { get; set; }

        [JsonPropertyName("public_key")]
        Guid PublicKey { get; set; }

        [JsonIgnore]
        int TenantId { get; }

        [JsonIgnore]
        string Log { get; }
    }
}
