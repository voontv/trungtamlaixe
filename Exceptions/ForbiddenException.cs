using System.Net;

namespace Ttlaixe.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string msg) : base(HttpStatusCode.Forbidden, msg)
        {
        }
    }
}