using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.Models;
using Ttlaixe.OracleBusinesses;
using Ttlaixe.Providers;
using Ttlaixe.LibsStartup;
using System.Text.RegularExpressions;
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
