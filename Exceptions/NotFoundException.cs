using System.Net;

namespace Ttlaixe.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string msg) : base(HttpStatusCode.NotFound, msg)
        {
        }
    }
}