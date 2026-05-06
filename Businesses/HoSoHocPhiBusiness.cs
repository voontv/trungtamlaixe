using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.LibsStartup;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(HoSoHocPhiBusiness))]
    public interface IHoSoHocPhiBusiness
    {
        Task<List<HoSoHocPhi>> GetAllAsync();
        Task<List<HoSoHocPhi>> GetChuaHoanThanhAsync();
        Task<List<HoSoHocPhi>> GetDaHoanThanhAsync();
        Task<List<HoSoHocPhi>> GetAllByMaKhoaHocsAsync(List<string> maKhoaHocs);
        Task<List<HoSoHocPhi>> GetChuaHoanThanhByMaKhoaHocsAsync(List<string> maKhoaHocs);
        Task<List<HoSoHocPhi>> GetDaHoanThanhByMaKhoaHocsAsync(List<string> maKhoaHocs);
        Task<HoSoHocPhi> CreateAsync(HoSoHocPhi model);
        Task<List<HoSoHocPhi>> CreateByKhoaHocAsync(string maKhoaHoc, string MahangGplx);
        Task<bool> UpdateAsync(string maDK, HoSoHocPhi model);
        Task<bool> UpdateTrangThaiThanhToanAsync(string maDK);
        Task<bool> BoHocAsync(string maDK);
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
                .Where(x => (bool)!x.BoHoc)
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        public async Task<List<HoSoHocPhi>> GetChuaHoanThanhAsync()
        {
            return await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x => (bool) !x.BoHoc && (bool)!x.DaHoanThanhHp)
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        public async Task<List<HoSoHocPhi>> GetDaHoanThanhAsync()
        {
            return await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x => (bool) !x.BoHoc && (bool)x.DaHoanThanhHp)
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        private async Task<HoSoHocPhi> BuildAsync(HoSoHocPhi model)
        {
            var existed = await _context.HoSoHocPhis
                .AnyAsync(x => x.MaDk == model.MaDk);

            if (existed)
                throw new Exception("Hồ sơ học phí này đã có.");

            var hocPhi = await _context.DmHocPhis
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaHangGplx == model.MaHangGplx);

            if (hocPhi == null)
                throw new Exception("Không tìm thấy học phí của hạng GPLX này.");

            model.HocPhi = hocPhi.HocPhi;
            model.DaHoanThanhHp = false;
            model.BoHoc = false;
            model.NgayKhoiTao = DateTime.Now;
            model.NgayChinhSuaCuoiCung = null;

            return model;
        }

        public async Task<HoSoHocPhi> CreateAsync(HoSoHocPhi model)
        {
            var entity = await BuildAsync(model);

            _context.HoSoHocPhis.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        public async Task<bool> UpdateAsync(string maDK, HoSoHocPhi model)
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

            hoSo.DaHoanThanhHp = tongDaNop >= hoSo.HocPhi;
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
            hoSo.NgayChinhSuaCuoiCung = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<HoSoHocPhi>> GetAllByMaKhoaHocsAsync(List<string> maKhoaHocs)
        {
            maKhoaHocs = maKhoaHocs
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            return await _context.HoSoHocPhis
                .AsNoTracking()
                .Where(x => (bool)!x.BoHoc && x.MaKhoaHoc != null && maKhoaHocs.Contains(x.MaKhoaHoc))
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
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
                    (bool) !x.BoHoc &&
                    (bool) x.DaHoanThanhHp &&
                    x.MaKhoaHoc != null &&
                    maKhoaHocs.Contains(x.MaKhoaHoc))
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();
        }

        public async Task<List<HoSoHocPhi>> CreateByKhoaHocAsync(string maKhoaHoc, string hangDt)
        {
            if (maKhoaHoc.Equals(Constants.MaKhoaHocTam))
            {
                return await CreateByKhoaHocTamAsync();
            }

            return await CreateByKhoaHocChuanAsync(maKhoaHoc, hangDt);
        }
        private async Task<List<HoSoHocPhi>> CreateByKhoaHocTamAsync()
        {
            var dsTam = await _context.HocVienChuaPhanKhoas
                .Where(x => x.TrangThai == true)
                .ToListAsync();

            var ids = dsTam.Select(x => x.IdHs.ToString()).ToList();

            // Kiểm tra hồ sơ đã tồn tại (dùng IdHs làm MaDk giả)
            var existedDict = await _context.HoSoHocPhis
                .Where(x => ids.Contains(x.MaDk))
                .ToDictionaryAsync(x => x.MaDk);

            var result = new List<HoSoHocPhi>();
            var toAdd = new List<HoSoHocPhi>();

            foreach (var hv in dsTam)
            {
                var maDkFake = hv.IdHs.ToString();

                if (existedDict.TryGetValue(maDkFake, out var existed))
                {
                    result.Add(existed);
                    continue;
                }

                // Lấy học phí theo HangDaoTao của học viên
                var hocPhi = await _context.DmHocPhis
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.MaHangGplx == hv.HangDaoTao);

                var model = new HoSoHocPhi
                {
                    MaDk = maDkFake,              // KEY QUAN TRỌNG
                    HoVaTen = $"{hv.HoDemNlx} {hv.TenNlx}",
                    NgaySinh = hv.NgaySinh.ToString("ddMMyyyy"),
                    MaHangGplx = hv.HangDaoTao,
                    HocPhi = hocPhi?.HocPhi ?? 0,
                    DaHoanThanhHp = false,
                    BoHoc = false,
                    NgayKhoiTao = DateTime.Now,
                    GioiTinh = hv.GioiTinh,
                    SoCmt = hv.SoCmt
                };

                toAdd.Add(model);
                result.Add(model);
            }

            if (toAdd.Any())
            {
                _context.HoSoHocPhis.AddRange(toAdd);
                await _context.SaveChangesAsync();
            }

            return result;
        }
        public async Task<List<HoSoHocPhi>> CreateByKhoaHocChuanAsync(string maKhoaHoc, string hangDt)
        {
            var dsHocViens = await _nguoiLxes.GetThongTinCoBanByKhoaHocAsync(maKhoaHoc);

            var maDks = dsHocViens.Select(x => x.MaDk).ToList();

            // Lấy sẵn những hồ sơ đã tồn tại
            var existedDict = await _context.HoSoHocPhis
                .Where(x => maDks.Contains(x.MaDk))
                .ToDictionaryAsync(x => x.MaDk);

            var hocPhi = await _context.DmHocPhis
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaHangGplx == hangDt);

            var result = new List<HoSoHocPhi>();
            var toAdd = new List<HoSoHocPhi>();

            foreach (var d in dsHocViens)
            {
                // ĐÃ CÓ → lấy ra dùng lại
                if (existedDict.TryGetValue(d.MaDk, out var existed))
                {
                    result.Add(existed);
                    continue;
                }

                // CHƯA CÓ → tạo mới
                var model = new HoSoHocPhi();
                d.Patch(model);

                model.MaHangGplx = hangDt;
                model.HocPhi = hocPhi.HocPhi;
                model.DaHoanThanhHp = false;
                model.BoHoc = false;
                model.NgayKhoiTao = DateTime.Now;

                toAdd.Add(model);
                result.Add(model);
            }

            if (toAdd.Any())
            {
                _context.HoSoHocPhis.AddRange(toAdd);
                await _context.SaveChangesAsync();
            }

            return result;
        }
    }
}
