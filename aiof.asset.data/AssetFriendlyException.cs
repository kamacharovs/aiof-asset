using System;
using System.Net;

namespace aiof.asset.data
{
    public class AssetFriendlyException : AssetException
    {
        public AssetFriendlyException()
        { }

        public AssetFriendlyException(string message)
            : base(message)
        { }

        public AssetFriendlyException(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        { }

        public AssetFriendlyException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
