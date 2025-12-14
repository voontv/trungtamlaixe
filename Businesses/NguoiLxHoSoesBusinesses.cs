using Microsoft.AspNetCore.Mvc;
using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using Ttlaixe.Providers;
namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(NguoiLxHoSoesBusinesses))]
    public interface INguoiLxHoSoesBusinesses
    {

    }

    public class NguoiLxHoSoesBusinesses: ControllerBase, INguoiLxHoSoesBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenInfo _authenInfo;
        public NguoiLxHoSoesBusinesses(GplxCsdtContext context,  IAuthenInfo authenInfo)
        {
            _context = context;
            _authenInfo = authenInfo;
        }

        
    }
}
