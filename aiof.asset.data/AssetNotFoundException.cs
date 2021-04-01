using System;
using System.Net;

namespace aiof.asset.data
{
    public class AssetNotFoundException : AssetFriendlyException
    {
        private const string DefaultMessage = "The requested item was not found.";

        public AssetNotFoundException()
            : base(HttpStatusCode.NotFound, DefaultMessage)
        { }

        public AssetNotFoundException(string message)
            : base(HttpStatusCode.NotFound, message)
        { }

        public AssetNotFoundException(string message, Exception inner)
            : base(message, inner)
        { }

        public AssetNotFoundException(Exception inner)
            : base(DefaultMessage, inner)
        { }
    }
}
