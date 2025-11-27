using System.Net;

namespace Ttlaixe.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string msg) : base(HttpStatusCode.BadRequest, msg)
        {
        }
    }
}