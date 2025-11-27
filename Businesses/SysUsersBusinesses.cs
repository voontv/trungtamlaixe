using BusinessLogic.Providers;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using Ttlaixe.Models;
using Ttlaixe.OracleBusinesses;
using Ttlaixe.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
