using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.Models;
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

        Task<object> GetThongTinKhoaHoc(string MaKhoaHoc);
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
            var thoiGiandt = await _context.DmHangDts.FindAsync(khoaHocRq.HangDt);
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
            khoaHoc.ThoiGianDt = thoiGiandt.ThoiGianDaoTao;
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

            // Lọc từ ngày
            if (dk.NgayBatDau.HasValue)
            {
                result = result.Where(x => x.NgayKg >= dk.NgayBatDau.Value);
            }

            // Lọc đến ngày (bao gồm cả NgayKetThuc → < NgayKetThuc + 1)
            if (dk.NgayKetThuc.HasValue)
            {
                var toDatePlus1 = dk.NgayKetThuc.Value.AddDays(1);
                result = result.Where(x => x.NgayKg < toDatePlus1);
            }

            // Các TT_XuLy hợp lệ
            var trangThaiHopLe = new[] { "01", "02", "03", "04" };

            // Loại KHÓA HỌC nào mà trong NguoiLx_HoSo có:
            // MaKhoaHoc trùng
            //   và (MaBC1 != null hoặc MaBC2 != null
            //        hoặc TT_XuLy không thuộc 01,02,03,04)
            result = result.Where(k => !_context.NguoiLxHoSos.Any(h =>
                h.MaKhoaHoc == k.MaKh &&
                (
                    h.MaBc1 != null ||                 // hoặc h.MaBC1 tùy tên entity
                    h.MaBc2 != null ||                 // hoặc h.MaBC2
                    !trangThaiHopLe.Contains(h.TtXuLy) // TT_XuLy != 01,02,03,04
                )));

            var khoaHocs = await result
                .OrderByDescending(x => x.NgayKg)
                .ToListAsync();

            var khoaHocRess = new List<KhoaHocResponse>();
            khoaHocs.Patch(khoaHocRess);

            return khoaHocRess;
        }



        private bool KhoaHocExists(string id)
        {
            return _context.KhoaHocs.Any(e => e.MaKh == id);
        }

        public async Task<object> GetThongTinKhoaHoc(string MaKhoaHoc)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(MaKhoaHoc);
            var result = new KhoaHocResponse();
            khoaHoc.Patch(result);
            return result;
        }
    }
}
