using System;
using System.Net;

namespace Ttlaixe.Exceptions
{
    public class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected BaseException(HttpStatusCode statusCode, string msg) : base(msg)
        {
            StatusCode = statusCode;
        }
    }
}