using Microsoft.AspNetCore.Http;
using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using System.Security.Claims;

namespace Ttlaixe.OracleBusinesses
{
    [ImplementBy(typeof(AuthenInfo))]
    public interface IAuthenInfo
    {
        LoggedInUser Get();
    }
    public class AuthenInfo : IAuthenInfo
    {
        private readonly IHttpContextAccessor context;

        public AuthenInfo(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public LoggedInUser Get()
        {
            if (context.HttpContext == null || context.HttpContext.User == null)
            {
                return null;
            }

            var userPrincipal = context.HttpContext.User;

            if (!userPrincipal.Identity.IsAuthenticated)
            {
                return null;
            }

            var maNhanVienClaim = userPrincipal.FindFirst(ClaimTypes.Name);

            if (maNhanVienClaim != null)
            {
                return new LoggedInUser
                {
                    IdHs = maNhanVienClaim.Value,
                };
            }
            //if (context.HttpContext.User is UserClaimsPrincipal user)
            //{
            //    return new LoggedInUser
            //    {
            //        IdHs = user.IDHoSoNhanVien,
            //    };
            //}

            return null;
        }
    }
}