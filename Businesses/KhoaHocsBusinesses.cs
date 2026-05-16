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
using DocumentFormat.OpenXml.Office2010.Excel;
namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(KhoaHocsBusinesses))]
    public interface IKhoaHocsBusinesses
    {
        //Task<KhoaHocResponse> PostKhoaHoc(KhoaHocCreateRequest khoaHoc);

        //Task PostKhoaHocTam();

        Task<List<KhoaHocResponse>> GetListKhoaHocsTheoTg(MocThoiGian dk);
        Task<List<KhoaHocResponse>> GetListKhoaHocsTheoHangMucDT(HangDaoTao dk);
        Task<List<KhoaHocResponse>> KhoaHocChuaTaoLichHoc();
        Task<object> GetThongTinKhoaHoc(string MaKhoaHoc);
    }

    public class KhoaHocsBusinesses : ControllerBase, IKhoaHocsBusinesses
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

        public async Task PostKhoaHocTam()
        {
            var maKh = Constants.MaKhoaHocTam;
            var heThong = await _context.QthtThamSoHts.Where(x => x.TenTs.Equals("MA_DONVI")).FirstOrDefaultAsync();
            if (await KhoaHocExistsAsync(maKh))
                throw new BadRequestException("Khóa học này đã được tạo");

            var khoaHoc = new KhoaHoc
            {
                MaSoGtvt = Constants.MaSoGTVT,
                MaCsdt = heThong.GiaTriTs,
                TenKh = heThong.GiaTriTs,
                MaKh = maKh,
                HangDt = "B.01",
                HangGplx = "B11"
            };

            _context.KhoaHocs.Add(khoaHoc);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlEx = ex.InnerException?.InnerException ?? ex.InnerException;

                var detail = $@"
                Lỗi khi lưu KhoaHoc:
                Message: {ex.Message}
                Inner: {ex.InnerException?.Message}
                Sql: {sqlEx?.Message}
                ";

                throw new BadRequestException(detail);
            }
        }

        private async Task<bool> KhoaHocExistsAsync(string maKh)
        {
            return await _context.KhoaHocs
                .AnyAsync(e => e.MaKh == maKh);
        }


        public async Task<List<KhoaHocResponse>> GetListKhoaHocsTheoHangMucDT(HangDaoTao dk)
        {
            var mocThoiGian = new MocThoiGian();
            mocThoiGian.NgayKetThuc = dk.NgayKetThuc;
            mocThoiGian.NgayBatDau = dk.NgayBatDau;
            var result = await GetListKhoaHocsTheoTg(mocThoiGian);
            return result.Where(x => x.HangDt == dk.HangDt).ToList();
        }

        public async Task<List<KhoaHocResponse>> GetListKhoaHocsTheoTg(MocThoiGian dk)
        {
            var exit = await KhoaHocExistsAsync(Constants.MaKhoaHocTam);
            if (!exit)
            {
                await PostKhoaHocTam();
            }    
            
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

        public async Task<object> GetThongTinKhoaHoc(string MaKhoaHoc)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(MaKhoaHoc);
            var result = new KhoaHocResponse();
            khoaHoc.Patch(result);
            return result;
        }

        public async Task<List<KhoaHocResponse>> KhoaHocChuaTaoLichHoc()
        {
            // cheat để test
            //var now = DateTime.Now.AddYears(-1);

            var khoaHocs = await _context.KhoaHocs
                .AsNoTracking()
                //.Where(kh => kh.NgayKg <= now)//chỗ này để sau check các khóa chưa có lịch
                .Where(kh => !_context.LichHocs.Any(lh => lh.MaKh == kh.MaKh))
                .ToListAsync();

            var result = new List<KhoaHocResponse>();
            khoaHocs.Patch(result);

            return result;
        }

    }
}
