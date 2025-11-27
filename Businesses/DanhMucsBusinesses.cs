using Dapper;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using Ttlaixe.Models;
using Ttlaixe.OracleBusinesses;
using Ttlaixe.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(DanhMucsBusinesses))]
    public interface IDanhMucsBusinesses
    {
        Task<List<DmDiemSatHach>> GetDmDiemSatHach(string hang);
        Task<List<DmHangDaoTaoResponse>> GetDmHangDaoTao();
    }
    public class DanhMucsBusinesses : ControllerBase, IDanhMucsBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenInfo _authenInfo;
        public DanhMucsBusinesses(GplxCsdtContext context, ITokenGenerator tokenGenerator, IAuthenInfo authenInfo)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _authenInfo = authenInfo;
        }
        
        public async Task<List<DmDiemSatHach>> GetDmDiemSatHach(string hang)
        {
            return await _context.DmDiemSatHaches.Where(x => x.Hang == hang).ToListAsync();
        }

        public async Task<List<DmHangDaoTaoResponse>> GetDmHangDaoTao()
        {
            return await _context.DmHangDts.Select(x => new DmHangDaoTaoResponse 
            {
                MaHangDt = x.MaHangDt,
                TenHangDt = x.TenHangDt,
                HangGplx = x.HangGplx,
                SoVbpl = x.SoVbpl,
                TuoiHv = x.TuoiHv,
                ThamNien = x.ThamNien
            })
                .ToListAsync();
        }
    }
}
