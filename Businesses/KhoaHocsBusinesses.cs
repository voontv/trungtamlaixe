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
using System.Collections.Generic;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(KhoaHocsBusinesses))]
    public interface IKhoaHocsBusinesses
    {
        Task<KhoaHocResponse> PostKhoaHoc(KhoaHocCreateRequest khoaHoc);

        Task<List<KhoaHocResponse>> GetListKhoaHocsTheoTg(MocThoiGian dk);
    }

    public class KhoaHocsBusinesses: ControllerBase, IKhoaHocsBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenInfo _authenInfo;
        public KhoaHocsBusinesses(GplxCsdtContext context, ITokenGenerator tokenGenerator, IAuthenInfo authenInfo)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _authenInfo = authenInfo;
        }

        public async Task<KhoaHocResponse> PostKhoaHoc(KhoaHocCreateRequest khoaHocRq)
        {
            var time = DateTime.Now;
            var maKh = Constants.MaCSDT+"K"+time.Year.ToString()[^2..];//lấy 2 ký tự cuối của năm hiện tại
            maKh+= Regex.Replace(khoaHocRq.HangDt, @"[^\w\s]", "");
            var soLopHienTaiDoiVoiHangDt = await _context.KhoaHocs
                .Where(x => x.MaCsdt == Constants.MaCSDT && x.HangDt == khoaHocRq.HangDt && x.NgayKg.Value.Year == DateTime.Now.Year)
                .CountAsync();
            maKh += (soLopHienTaiDoiVoiHangDt + 1);
            var khoaHoc = new KhoaHoc();
            khoaHocRq.Patch(khoaHoc);
            khoaHoc.NgayTao = time;
            khoaHoc.NgaySua = khoaHoc.NgayTao;
            khoaHoc.MaKh = maKh;

            _context.KhoaHocs.Add(khoaHoc);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (KhoaHocExists(khoaHoc.MaKh))
                {
                     Conflict();
                }
                else
                {
                    throw new BadRequestException("Error found is "+e.Message);
                }
            }
            var khoaHocRes = new KhoaHocResponse();
            khoaHoc.Patch(khoaHocRes);

           return khoaHocRes;
        }

        public async Task<List<KhoaHocResponse>> GetListKhoaHocsTheoTg(MocThoiGian dk)
        {
            var result = _context.KhoaHocs.AsQueryable();
            if(dk.NgayBatDau.HasValue)
            {
                result = result.Where(x => x.NgayKg >= dk.NgayBatDau.Value);
            }

            if (dk.NgayBatDau.HasValue)
            {
                result = result.Where(x => x.NgayKg < dk.NgayKetThuc.Value.AddDays(1));
            }
            var khoaHocs = await result.OrderByDescending(x => x.NgayKg).ToListAsync();
            var khoaHocRess = new List<KhoaHocResponse>();
            khoaHocs.Patch(khoaHocRess);

            return khoaHocRess;
        }

        private bool KhoaHocExists(string id)
        {
            return _context.KhoaHocs.Any(e => e.MaKh == id);
        }
    }
}
