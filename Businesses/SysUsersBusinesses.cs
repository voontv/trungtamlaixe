using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using Ttlaixe.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(SysUsersBusinesses))]
    public interface ISysUsersBusinesses
    {
    }
    public class SysUsersBusinesses : ControllerBase, ISysUsersBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenInfo _authenInfo;

        public SysUsersBusinesses(GplxCsdtContext context, ITokenGenerator tokenGenerator, IAuthenInfo authenInfo)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _authenInfo = authenInfo;

        }
        
    }
}
