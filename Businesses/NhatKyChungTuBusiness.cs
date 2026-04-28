using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using Ttlaixe.DTO.response;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(NhatKyChungTuBusiness))]
    public interface INhatKyChungTuBusiness
    {
        Task<List<NhatKyChungTu>> GetAllAsync();
        Task<NhatKyChungTu?> GetByIdAsync(int idChungTu);
        Task<NhatKyChungTu> CreateAsync(NhatKyChungTu model);
        Task<bool> UpdateAsync(int idChungTu, NhatKyChungTu model);
        Task<bool> DeleteAsync(int idChungTu);

        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync();
        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync();
        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync(DateTime? fromDate, DateTime? toDate);
    }

    public class NhatKyChungTuBusiness : INhatKyChungTuBusiness
    {
        private readonly TeknovaContext _context;

        public NhatKyChungTuBusiness(TeknovaContext context)
        {
            _context = context;
        }

        public async Task<List<NhatKyChungTu>> GetAllAsync()
        {
            return await _context.NhatKyChungTus
                .AsNoTracking()
                .OrderByDescending(x => x.NgayLap)
                .ThenByDescending(x => x.IdChungTu)
                .ToListAsync();
        }

        public async Task<NhatKyChungTu?> GetByIdAsync(int idChungTu)
        {
            return await _context.NhatKyChungTus
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdChungTu == idChungTu);
        }

        public async Task<NhatKyChungTu> CreateAsync(NhatKyChungTu model)
        {
            var tkNoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanNo);

            if (!tkNoExists)
                throw new Exception("Tài khoản nợ không tồn tại.");

            var tkCoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanCo);

            if (!tkCoExists)
                throw new Exception("Tài khoản có không tồn tại.");

            if (model.SoTien <= 0)
                throw new Exception("Số tiền phải lớn hơn 0.");

            model.NgayKhoiTao = DateTime.Now;

            _context.NhatKyChungTus.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<bool> UpdateAsync(int idChungTu, NhatKyChungTu model)
        {
            var existing = await _context.NhatKyChungTus
                .FirstOrDefaultAsync(x => x.IdChungTu == idChungTu);

            if (existing == null)
                return false;

            var tkNoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanNo);

            if (!tkNoExists)
                throw new Exception("Tài khoản nợ không tồn tại.");

            var tkCoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanCo);

            if (!tkCoExists)
                throw new Exception("Tài khoản có không tồn tại.");

            if (model.SoTien <= 0)
                throw new Exception("Số tiền phải lớn hơn 0.");

            existing.SoChungTu = model.SoChungTu;
            existing.NgayLap = model.NgayLap;
            existing.DienGiai = model.DienGiai;
            existing.TaiKhoanNo = model.TaiKhoanNo;
            existing.TaiKhoanCo = model.TaiKhoanCo;
            existing.SoTien = model.SoTien;
            existing.GhiChu = model.GhiChu;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int idChungTu)
        {
            var existing = await _context.NhatKyChungTus
                .FirstOrDefaultAsync(x => x.IdChungTu == idChungTu);

            if (existing == null)
                return false;

            _context.NhatKyChungTus.Remove(existing);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync()
        {
            var noQuery =
                from nk in _context.NhatKyChungTus
                group nk by nk.TaiKhoanNo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.SoTien),
                    TongCo = 0m
                };

            var coQuery =
                from nk in _context.NhatKyChungTus
                group nk by nk.TaiKhoanCo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = 0m,
                    TongCo = g.Sum(x => x.SoTien)
                };

            var data = await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();

            return data;
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync()
        {
            var noQuery =
                from nk in _context.NhatKyChungTus
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanNo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = nk.SoTien,
                    TongCo = 0m
                };

            var coQuery =
                from nk in _context.NhatKyChungTus
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanCo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = 0m,
                    TongCo = nk.SoTien
                };

            var data = await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();

            return data;
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.NhatKyChungTus.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.NgayLap >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.NgayLap <= toDate.Value.Date);

            var noQuery =
                from nk in query
                group nk by nk.TaiKhoanNo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.SoTien),
                    TongCo = 0m
                };

            var coQuery =
                from nk in query
                group nk by nk.TaiKhoanCo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = 0m,
                    TongCo = g.Sum(x => x.SoTien)
                };

            return await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.NhatKyChungTus.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.NgayLap >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.NgayLap <= toDate.Value.Date);

            var noQuery =
                from nk in query
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanNo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = nk.SoTien,
                    TongCo = 0m
                };

            var coQuery =
                from nk in query
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanCo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = 0m,
                    TongCo = nk.SoTien
                };

            return await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();
        }
    }
}
