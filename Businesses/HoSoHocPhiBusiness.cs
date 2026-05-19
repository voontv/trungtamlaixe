using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using Ttlaixe.LibsStartup;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(HoSoHocPhiBusiness))]
    public interface IHoSoHocPhiBusiness
    {
        //Task<List<HoSoHocPhi>> GetAllAsync();
        Task<List<HoSoHocPhiResponse>> GetChuaHoanThanhAsync();
        //Task<List<HoSoHocPhi>> GetDaHoanThanhAsync();
        Task<List<HoSoHocPhiResponse>> GetAllByMaKhoaHocsAsync(List<string> maKhoaHocs);
        //Task<List<HoSoHocPhi>> GetChuaHoanThanhByMaKhoaHocsAsync(List<string> maKhoaHocs);
        //Task<List<HoSoHocPhi>> GetDaHoanThanhByMaKhoaHocsAsync(List<string> maKhoaHocs);
        Task<HoSoHocPhi> CreateAsync(HoSoHocPhiCreated model);
        //Task<List<HoSoHocPhi>> CreateByKhoaHocAsync(string maKhoaHoc, string MahangGplx);
        Task<bool> UpdateAsync(string maDK, HoSoHocPhiUpdate model);
        Task<bool> UpdateTrangThaiThanhToanAsync(string maDK);
        Task<bool> BoHocAsync(string maDK);

        //Task<List<HoSoHocPhi>> HoSoChuaNopHocPhi();

        Task DeleteHoSoHocPhi(string maDk);
    }

    public class HoSoHocPhiBusiness : IHoSoHocPhiBusiness
    {
        private readonly TeknovaContext _context;
        private readonly INguoiLxesBusinesses _nguoiLxes;
        public HoSoHocPhiBusiness(TeknovaContext context, INguoiLxesBusinesses nguoiLxes)
        {
            _context = context;
            _nguoiLxes = nguoiLxes;
        }

        public async Task<List<HoSoHocPhi>> GetAllAsync()
        {
            return await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x => (bool)!x.BoHoc && x.IsActive == true)
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        public async Task<List<HoSoHocPhiResponse>> GetChuaHoanThanhAsync()
        {
            var hoSoHocPhis = await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x => (bool)!x.BoHoc && (bool)!x.DaHoanThanhHp && x.IsActive == true)
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
            var result = new List<HoSoHocPhiResponse>();
            hoSoHocPhis.Patch(result);
            return result;
        }

        public async Task<List<HoSoHocPhi>> GetDaHoanThanhAsync()
        {
            return await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x => (bool)!x.BoHoc && (bool)x.DaHoanThanhHp)
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        private async Task<HoSoHocPhi> BuildAsync(HoSoHocPhiCreated model)
        {
            var existed = false;

            if(model.RotHocLaiCungHangLx == null || model.RotHocLaiCungHangLx == false)
            {
                existed = await _context.HoSoHocPhis
                .AnyAsync(x => x.MaHangGplx == model.MaHangGplx && model.SoCmt == x.SoCmt);
            }    

            if (existed)
                throw new BadRequestException("Hồ sơ học phí này đã có.");

            var hocPhi = await _context.DmHocPhis
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaHangGplx == model.MaHangGplx);

            if (hocPhi == null)
                throw new BadRequestException("Không tìm thấy học phí của hạng GPLX này.");

            var hoSoHocPhi = new HoSoHocPhi();
            model.Patch(hoSoHocPhi);

            return hoSoHocPhi;
        }

        public async Task<HoSoHocPhi> CreateAsync(HoSoHocPhiCreated model)
        {
            var entity = await BuildAsync(model);

            _context.HoSoHocPhis.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        public async Task<bool> UpdateAsync(string maDK, HoSoHocPhiUpdate model)
        {
            var existing = await _context.HoSoHocPhis
                .FirstOrDefaultAsync(x => x.MaDk == maDK);

            if (existing == null)
                return false;

            existing.MaKhoaHoc = model.MaKhoaHoc;
            existing.MaHangGplx = model.MaHangGplx;
            existing.HoVaTen = model.HoVaTen;
            existing.NgaySinh = model.NgaySinh;
            existing.SoCmt = model.SoCmt;
            existing.GioiTinh = model.GioiTinh;
            existing.NoiCuTru = model.NoiCuTru;
            existing.NoiThuongTru = model.NoiThuongTru;
            existing.NgayChinhSuaCuoiCung = DateTime.Now;

            var hocPhi = await _context.DmHocPhis
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaHangGplx == model.MaHangGplx);

            if (hocPhi != null)
                existing.HocPhi = hocPhi.HocPhi;

            await _context.SaveChangesAsync();

            await UpdateTrangThaiThanhToanAsync(maDK);

            return true;
        }

        public async Task<bool> UpdateTrangThaiThanhToanAsync(string maDK)
        {
            var hoSo = await _context.HoSoHocPhis
                .FirstOrDefaultAsync(x => x.MaDk == maDK);

            if (hoSo == null)
                return false;

            var tongDaNop = await _context.LichSuNopHocPhis
                .Where(x => x.MaDk == maDK)
                .SumAsync(x => (decimal?)x.SoTienNop) ?? 0;
            if (hoSo.HocPhi == tongDaNop)
            {
                hoSo.DaHoanThanhHp = true;
            } 
            else if(hoSo.HocPhi > tongDaNop)
            {
                hoSo.DaHoanThanhHp = false;
            }   
            else
            {
                throw new BadRequestException("Vui lòng kiểm tra lại số tiền đã nộp. Vì hiện tại tổng số tiền đã nộp là "
                    + tongDaNop + " lớn hơn số tiền học phí cập nhật.");
            }
           
            hoSo.NgayChinhSuaCuoiCung = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BoHocAsync(string maDK)
        {
            var hoSo = await _context.HoSoHocPhis
                .FirstOrDefaultAsync(x => x.MaDk == maDK);

            if (hoSo == null)
                return false;

            hoSo.BoHoc = true;
            hoSo.IsActive = false;
            hoSo.NgayChinhSuaCuoiCung = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<HoSoHocPhiResponse>> GetAllByMaKhoaHocsAsync(List<string> maKhoaHocs)
        {
            maKhoaHocs = maKhoaHocs
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var hoSoHocPhis = await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x => (bool)!x.BoHoc && x.MaKhoaHoc != null && maKhoaHocs.Contains(x.MaKhoaHoc))
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();

            var result = new List<HoSoHocPhiResponse>();
            hoSoHocPhis.Patch(result);
            return result;
        }

        public async Task<List<HoSoHocPhi>> GetChuaHoanThanhByMaKhoaHocsAsync(List<string> maKhoaHocs)
        {
            maKhoaHocs = maKhoaHocs
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            return await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x =>
                    (bool)!x.BoHoc &&
                    (bool)!x.DaHoanThanhHp &&
                    x.MaKhoaHoc != null &&
                    maKhoaHocs.Contains(x.MaKhoaHoc))
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        public async Task<List<HoSoHocPhi>> GetDaHoanThanhByMaKhoaHocsAsync(List<string> maKhoaHocs)
        {
            maKhoaHocs = maKhoaHocs
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            return await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x =>
                    (bool)!x.BoHoc &&
                    (bool)x.DaHoanThanhHp &&
                    x.MaKhoaHoc != null &&
                    maKhoaHocs.Contains(x.MaKhoaHoc))
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        //public async Task<List<HoSoHocPhi>> CreateByKhoaHocAsync(string maKhoaHoc, string hangDt)
        //{
        //    //if (!maKhoaHoc.Equals(Constants.MaKhoaHocTam))
        //    //{
        //    //    await CreateByKhoaHocChuanAsync(maKhoaHoc, hangDt);
        //    //}

            
        //    return await CreateByKhoaHocTamAsync();
        //}

        //public async Task<List<HoSoHocPhi>> HoSoChuaNopHocPhi()
        //{
        //    var thoiGian = new MocThoiGian();
        //    var khoaHocs = await _khoaHocs.GetListKhoaHocsTheoTg(thoiGian);
        //    var hoSoHocPhis = new List<HoSoHocPhi>();
        //    foreach(var khoaHoc in khoaHocs)
        //    {
        //        var hoSoHocPhi = await CreateByKhoaHocAsync(khoaHoc.MaKh, khoaHoc.HangDt);
        //        hoSoHocPhis.AddRange(hoSoHocPhi);
        //    }

        //    return hoSoHocPhis;
        //}
         
        //private async Task<List<HoSoHocPhi>> CreateByKhoaHocTamAsync()
        //{
        //    var dsTam = await _context.HocVienChuaPhanKhoas
        //        .Where(x => x.TrangThai == true)
        //        .ToListAsync();

        //    var ids = dsTam.Select(x => x.IdHs.ToString()).ToList();

        //    // Kiểm tra hồ sơ đã tồn tại (dùng IdHs làm MaDk giả)
        //    var existedDict = await _context.HoSoHocPhis
        //        .Where(x => ids.Contains(x.MaDk))
        //        .ToDictionaryAsync(x => x.MaDk);

        //    var result = new List<HoSoHocPhi>();
        //    var toAdd = new List<HoSoHocPhi>();

        //    foreach (var hv in dsTam)
        //    {
        //        var maDkFake = hv.IdHs.ToString();

        //        if (existedDict.TryGetValue(maDkFake, out var existed))
        //        {
        //            result.Add(existed);
        //            continue;
        //        }

        //        // Lấy học phí theo HangDaoTao của học viên
        //        var hocPhi = await _context.DmHocPhis
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync(x => x.MaHangGplx == hv.HangDaoTao);

        //        var model = new HoSoHocPhi
        //        {
        //            MaDk = maDkFake,              // KEY QUAN TRỌNG
        //            HoVaTen = $"{hv.HoDemNlx} {hv.TenNlx}",
        //            NgaySinh = hv.NgaySinh.ToString("ddMMyyyy"),
        //            MaHangGplx = hv.HangDaoTao,
        //            HocPhi = hocPhi?.HocPhi ?? 0,
        //            DaHoanThanhHp = false,
        //            BoHoc = false,
        //            NgayKhoiTao = DateTime.Now,
        //            GioiTinh = hv.GioiTinh,
        //            SoCmt = hv.SoCmt
        //        };

        //        toAdd.Add(model);
        //        result.Add(model);
        //    }

        //    if (toAdd.Any())
        //    {
        //        _context.HoSoHocPhis.AddRange(toAdd);
        //        await _context.SaveChangesAsync();
        //    }

        //    return result;
        //}
        public async Task CreateByKhoaHocChuanAsync(string maKhoaHoc, string hangDt)
        {
            var dsHocViens = await _nguoiLxes.GetThongTinCoBanByKhoaHocAsync(maKhoaHoc);

            if (!dsHocViens.Any())
                return;

            // Lấy danh sách MaDk đã tồn tại trong HocVienChuaPhanKhoa của khóa này
            var existedMaDk = await _context.HocVienChuaPhanKhoas
                .Where(x => x.MaKhoaHoc == maKhoaHoc)
                .Select(x => x.MaDk)
                .ToListAsync();

            var existedSet = existedMaDk.ToHashSet();

            // Lọc ra những người CHƯA có
            var needInsert = dsHocViens
                .Where(x => !existedSet.Contains(x.MaDk))
                .Select(x => new HocVienChuaPhanKhoa
                {
                    HoDemNlx = x.HoDemNlx,
                    TenNlx = x.TenNlx,
                    MaQuocTich = x.MaQuocTich,
                    NgaySinh = DateTime.ParseExact(x.NgaySinh, "yyyyMMdd",CultureInfo.InvariantCulture),
                    SoCmt = x.SoCmt,
                    MaDk = x.MaDk,
                    MaKhoaHoc = maKhoaHoc,
                    HangDaoTao = hangDt,
                    TrangThai = false
                })
                .ToList();

            if (needInsert.Any())
            {
                await _context.HocVienChuaPhanKhoas.AddRangeAsync(needInsert);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteHoSoHocPhi(string maDk)
        {
           
            var hoSoHp = await _context.HoSoHocPhis.FindAsync(maDk) ?? throw new BadRequestException("Không tìm thấy mã "+maDk);
            hoSoHp.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
