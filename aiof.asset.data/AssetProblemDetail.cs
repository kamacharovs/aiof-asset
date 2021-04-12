using System.Collections.Generic;

namespace aiof.asset.data
{
    public class AssetProblemDetail : IAssetProblemDetail
    {
        public int? Code { get; set; }
        public string Message { get; set; }
        public string TraceId { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class AssetProblemDetailBase : IAssetProblemDetailBase
    {
        public int? Code { get; set; }
        public string Message { get; set; }
    }
}
