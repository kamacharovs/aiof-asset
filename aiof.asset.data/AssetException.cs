using System;
using System.Net;
using System.Text.Json;

namespace aiof.asset.data
{
    public abstract class AssetException : ApplicationException
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; }

        protected AssetException()
        { }

        protected AssetException(int statusCode)
        {
            StatusCode = statusCode;
        }

        protected AssetException(string message)
            : base(message)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        protected AssetException(string message, Exception inner)
            : base(message, inner)
        { }

        protected AssetException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        protected AssetException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = (int)statusCode;
        }

        protected AssetException(int statusCode, Exception inner)
            : this(statusCode, inner.ToString())
        { }

        protected AssetException(HttpStatusCode statusCode, Exception inner)
            : this(statusCode, inner.ToString())
        { }

        protected AssetException(int statusCode, JsonElement errorObject)
            : this(statusCode, errorObject.ToString())
        {
            ContentType = @"application/problem+json";
        }
    }
}
