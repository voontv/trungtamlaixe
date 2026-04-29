using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.WebSockets;

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

        public async Task<List<HoSoHocPhi>> CreateByKhoaHocAsync(string maKhoaHoc, string MahangGplx)
        {
            var dsHocViens = await _nguoiLxes.GetThongTinCoBanByKhoaHocAsync(maKhoaHoc);

            var list = new List<HoSoHocPhi>();

            foreach (var d in dsHocViens)
            {
                var model = new HoSoHocPhi();
                d.Patch(model);
                model.MaHangGplx = MahangGplx;
                var entity = await BuildAsync(model);
                list.Add(entity);
            }

            _context.HoSoHocPhis.AddRange(list);
            await _context.SaveChangesAsync();

            return list;
        }
    }
}
