using System.Collections.Generic;

namespace aiof.asset.data
{
    public interface IAssetProblemDetail
    {
        int? Code { get; set; }
        string Message { get; set; }
        string TraceId { get; set; }
        IEnumerable<string> Errors { get; set; }
    }

    public interface IAssetProblemDetailBase
    {
        int? Code { get; set; }
        string Message { get; set; }
    }
}
