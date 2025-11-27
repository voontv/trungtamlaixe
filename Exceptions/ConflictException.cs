using System.Net;

namespace Ttlaixe.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(string msg) : base(HttpStatusCode.Conflict, msg)
        {
        }
    }
}