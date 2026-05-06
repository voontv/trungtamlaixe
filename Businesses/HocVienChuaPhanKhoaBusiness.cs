using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(HocVienChuaPhanKhoaBusiness))]
    public interface IHocVienChuaPhanKhoaBusiness
    {
        Task<List<HocVienChuaPhanKhoaDTO>> GetAllAsync();
        Task<HocVienChuaPhanKhoa> CreateAsync(HocVienChuaPhanKhoa model, IFormFile file);
        Task<bool> UpdateAsync(HocVienChuaPhanKhoa model);
        Task<bool> DeleteAsync(int id);
        Task<List<HocVienChuaPhanKhoaDTO>> SearchAsync(HocVienChuaPhanKhoaSearchRequest rq);
    }

    public class HocVienChuaPhanKhoaBusiness : IHocVienChuaPhanKhoaBusiness
    {
        private readonly TeknovaContext _context;
        private readonly GplxCsdtContext _gplx;

        public HocVienChuaPhanKhoaBusiness(TeknovaContext context, GplxCsdtContext gplx)
        {
            _context = context;
            _gplx = gplx;
        }

        public async Task<List<HocVienChuaPhanKhoaDTO>> GetAllAsync()
        {
            var gvDict = await _gplx.GiaoViens
            .Select(x => new { x.MaGv, x.TenGv, x.HoTenDem })
            .ToDictionaryAsync(x => x.MaGv);
            var hvs = await _context.HocVienChuaPhanKhoas
            .OrderByDescending(x => x.NgayNopHoSo)
            .ToListAsync();
            var result = hvs.Select(hv =>
            {
                gvDict.TryGetValue(hv.MaGv, out var gv);

                return new HocVienChuaPhanKhoaDTO
                {
                    HocVien = hv,
                    TenGv = gv?.TenGv,
                    HoTenDem = gv?.HoTenDem
                };
            }).ToList();
            return result;
        }
        public async Task<HocVienChuaPhanKhoa> CreateAsync(HocVienChuaPhanKhoa model, IFormFile file)
        {
            var thamSoHt = await _gplx.QthtThamSoHts
                .FirstOrDefaultAsync(x => x.TenTs == "IMG_PATH_CSDT");

            var image_path = thamSoHt?.GiaTriTs ?? @"\\192.168.100.248\d\2026\im_gplx";

            var resolver = new ImagePathResolver(image_path);

            model.NgayNopHoSo = DateTime.Now;

            _context.HocVienChuaPhanKhoas.Add(model);
            await _context.SaveChangesAsync();

            // ===== Build đường dẫn động từ resolver =====
            var year = DateTime.Now.Year.ToString();

            var localFolder = Path.Combine(
                resolver.LocalRoot,   // D:\
                year,
                resolver.BaseFolder,  // im_gplx
                model.SoCmt
            );

            Directory.CreateDirectory(localFolder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{model.SoCmt}-{DateTime.Now:yyyyMMdd-HHmmss}{ext}";
            var localFullPath = Path.Combine(localFolder, fileName);

            using (var stream = new FileStream(localFullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ===== Lưu UNC path cho desktop =====
            model.DuongDanAnh = Path.Combine(
                resolver.UncRoot,     // \\ip\d\
                year,
                resolver.BaseFolder,
                model.SoCmt,
                fileName
            );

            await _context.SaveChangesAsync();

            return model;
        }

        public async Task UpdateAnhAsync(int idHs, IFormFile file)
        {
            var hv = await _context.HocVienChuaPhanKhoas
                .FirstOrDefaultAsync(x => x.IdHs == idHs);

            if (hv == null)
                throw new Exception("Không tìm thấy hồ sơ.");

            // ==== Lấy UNC gốc từ DB ====
            var thamSoHt = await _gplx.QthtThamSoHts
                .FirstOrDefaultAsync(x => x.TenTs == "IMG_PATH_CSDT");

            var image_path = thamSoHt?.GiaTriTs
                ?? @"\\192.168.100.248\d\2026\im_gplx";

            var resolver = new ImagePathResolver(image_path);

            // ==== Build path động ====
            var year = DateTime.Now.Year.ToString();

            var localFolder = Path.Combine(
                resolver.LocalRoot,   // D:\
                year,
                resolver.BaseFolder,  // im_gplx
                idHs.ToString()
            );

            Directory.CreateDirectory(localFolder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{idHs}-{DateTime.Now:yyyyMMdd-HHmmss}{ext}";
            var localFullPath = Path.Combine(localFolder, fileName);

            using (var stream = new FileStream(localFullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ==== Lưu UNC cho desktop đọc ====
            hv.DuongDanAnh = Path.Combine(
                resolver.UncRoot,     // \\ip\d\
                year,
                resolver.BaseFolder,
                idHs.ToString(),
                fileName
            );

            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(HocVienChuaPhanKhoa model)
        {
            var entity = await _context.HocVienChuaPhanKhoas.FindAsync(model.IdHs);
            if (entity == null) return false;

            _context.Entry(entity).CurrentValues.SetValues(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.HocVienChuaPhanKhoas.FindAsync(id);
            if (entity == null) return false;

            _context.HocVienChuaPhanKhoas.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<HocVienChuaPhanKhoaDTO>> SearchAsync(HocVienChuaPhanKhoaSearchRequest rq)
        {
            // 1) Chuẩn bị dictionary giáo viên
            var gvQuery = _gplx.GiaoViens.AsQueryable();

            if (!string.IsNullOrWhiteSpace(rq.TenGiaoVien))
            {
                gvQuery = gvQuery.Where(x =>
                    (x.HoTenDem + " " + x.TenGv).Contains(rq.TenGiaoVien));
            }

            var gvList = await gvQuery
                .Select(x => new { x.MaGv, x.TenGv, x.HoTenDem })
                .ToListAsync();

            var gvDict = gvList.ToDictionary(x => x.MaGv);

            // Nếu có lọc theo tên GV thì tạo tập MaGv để filter HV
            var maGvFilter = gvList.Select(x => x.MaGv).ToHashSet();

            // 2) Query học viên
            var query = _context.HocVienChuaPhanKhoas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(rq.HoVaTen))
            {
                query = query.Where(x =>
                    (x.HoDemNlx + " " + x.TenNlx).Contains(rq.HoVaTen));
            }

            if (!string.IsNullOrWhiteSpace(rq.MaQuocTich))
                query = query.Where(x => x.MaQuocTich == rq.MaQuocTich);

            if (!string.IsNullOrWhiteSpace(rq.HangDaoTao))
                query = query.Where(x => x.HangDaoTao == rq.HangDaoTao);

            if (!string.IsNullOrWhiteSpace(rq.SoDienThoai))
                query = query.Where(x => x.SoDienThoai.Contains(rq.SoDienThoai));

            // Filter theo MaGV hoặc theo TenGV
            if (!string.IsNullOrWhiteSpace(rq.MaGV))
            {
                query = query.Where(x => x.MaGv == rq.MaGV);
            }
            else if (!string.IsNullOrWhiteSpace(rq.TenGiaoVien))
            {
                query = query.Where(x => maGvFilter.Contains(x.MaGv));
            }

            if (rq.NgaySinhFrom.HasValue)
                query = query.Where(x => x.NgaySinh >= rq.NgaySinhFrom.Value);

            if (rq.NgaySinhTo.HasValue)
                query = query.Where(x => x.NgaySinh <= rq.NgaySinhTo.Value);

            if (rq.NgayNopHoSoFrom.HasValue)
                query = query.Where(x => x.NgayNopHoSo >= rq.NgayNopHoSoFrom.Value);

            if (rq.NgayNopHoSoTo.HasValue)
                query = query.Where(x => x.NgayNopHoSo <= rq.NgayNopHoSoTo.Value);

            if (rq.SoTienNopFrom.HasValue)
                query = query.Where(x => x.SoTienNop >= rq.SoTienNopFrom.Value);

            if (rq.SoTienNopTo.HasValue)
                query = query.Where(x => x.SoTienNop <= rq.SoTienNopTo.Value);

            if (rq.CamKet.HasValue)
                query = query.Where(x => x.CamKet == rq.CamKet);

            if (rq.AnhThe.HasValue)
                query = query.Where(x => x.AnhThe == rq.AnhThe);

            if (rq.Don.HasValue)
                query = query.Where(x => x.Don == rq.Don);

            if (rq.HopDong.HasValue)
                query = query.Where(x => x.HopDong == rq.HopDong);

            if (rq.DonSatHach.HasValue)
                query = query.Where(x => x.DonSatHach == rq.DonSatHach);

            if (rq.GKSK.HasValue)
                query = query.Where(x => x.Gksk == rq.GKSK);

            if (rq.VanTayKhuonMat.HasValue)
                query = query.Where(x => x.VanTayKhuonMat == rq.VanTayKhuonMat);

            if (rq.ChupAnh.HasValue)
                query = query.Where(x => x.ChupAnh == rq.ChupAnh);

            var hvs = await query
                .OrderByDescending(x => x.NgayNopHoSo)
                .ToListAsync();

            // 3) Map sang DTO kèm tên GV
            var result = hvs.Select(hv =>
            {
                gvDict.TryGetValue(hv.MaGv, out var gv);

                return new HocVienChuaPhanKhoaDTO
                {
                    HocVien = hv,
                    TenGv = gv?.TenGv,
                    HoTenDem = gv?.HoTenDem
                };
            }).ToList();

            return result;
        }

        public async Task RebuildImagePathAsync(int idHs)
        {
            var hv = await _context.HocVienChuaPhanKhoas
                .FirstOrDefaultAsync(x => x.IdHs == idHs);

            if (hv == null || string.IsNullOrWhiteSpace(hv.DuongDanAnh))
                return;

            // ==== Lấy cấu hình mới nhất ====
            var ts = await _gplx.QthtThamSoHts
                .FirstOrDefaultAsync(x => x.TenTs == "IMG_PATH_CSDT");

            var resolver = new ImagePathResolver(ts.GiaTriTs);

            // ==== Convert UNC cũ -> local cũ ====
            var oldLocalPath = hv.DuongDanAnh
                .Replace(resolver.UncRoot, resolver.LocalRoot);

            if (!File.Exists(oldLocalPath))
                return;

            // ==== Rule mới (ví dụ đổi theo SoCmt) ====
            var year = DateTime.Now.Year.ToString();

            var newFolder = Path.Combine(
                resolver.LocalRoot,
                year,
                resolver.BaseFolder,
                hv.SoCmt     // đổi rule tại đây
            );

            Directory.CreateDirectory(newFolder);

            var ext = Path.GetExtension(oldLocalPath);
            var newFileName = $"{hv.SoCmt}-{DateTime.Now:yyyyMMddHHmmss}{ext}";
            var newLocalPath = Path.Combine(newFolder, newFileName);

            File.Move(oldLocalPath, newLocalPath);

            // ==== Update UNC mới ====
            hv.DuongDanAnh = Path.Combine(
                resolver.UncRoot,
                year,
                resolver.BaseFolder,
                hv.SoCmt,
                newFileName
            );

            await _context.SaveChangesAsync();
        }

    }
}
