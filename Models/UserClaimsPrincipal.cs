using System.Security.Claims;

namespace Ttlaixe.OracleModels
{
    public class UserClaimsPrincipal : ClaimsPrincipal
    {
        public UserClaimsPrincipal(ClaimsIdentity claimsIdentity) : base(claimsIdentity)
        {
            IDHoSoNhanVien = claimsIdentity.Name;
        }

        public string IDHoSoNhanVien { get; }
    }
}