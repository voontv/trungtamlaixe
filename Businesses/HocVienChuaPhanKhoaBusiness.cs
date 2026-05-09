using DocumentFormat.OpenXml.InkML;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using Ttlaixe.LibsStartup;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(HocVienChuaPhanKhoaBusiness))]
    public interface IHocVienChuaPhanKhoaBusiness
    {
        Task<List<HocVienChuaPhanKhoaDTO>> GetAllAsync(bool? chuaCoLop);
        Task CreateAsync(HocVienChuaPhanKhoaRequest model);
        Task<bool> UpdateAsync(HocVienChuaPhanKhoaRequest model);
        Task<bool> DeleteAsync(int id);
        Task<List<HocVienChuaPhanKhoaDTO>> SearchAsync(HocVienChuaPhanKhoaSearchRequest rq);

        Task<(byte[] Bytes, string ContentType)?> GetImageByPathAsync(string uncPath);

        Task UpdateTrangThai(int IdHs);
        Task ChuyenLop(NguoiLxCreateRequest request);
    }

    public class HocVienChuaPhanKhoaBusiness : IHocVienChuaPhanKhoaBusiness
    {
        private readonly TeknovaContext _context;
        private readonly GplxCsdtContext _gplx;
        private readonly IHttpContextAccessor _http;
        private readonly INguoiLxesBusinesses _nguoiLxes;
        public HocVienChuaPhanKhoaBusiness(TeknovaContext context, GplxCsdtContext gplx, IHttpContextAccessor http, INguoiLxesBusinesses nguoiLxes)
        {
            _context = context;
            _gplx = gplx;
            _http = http;
            _nguoiLxes = nguoiLxes;
        }

        public async Task<List<HocVienChuaPhanKhoaDTO>> GetAllAsync(bool? chuaCoLop = true)
        {
            var gvDict = await _gplx.GiaoViens
                .Select(x => new { x.MaGv, x.TenGv, x.HoTenDem })
                .ToDictionaryAsync(x => x.MaGv);
            var hvs = await _context.HocVienChuaPhanKhoas
            .Where(x => x.TrangThai == (chuaCoLop ?? true))
            .OrderByDescending(x => x.NgayNopHoSo)
            .ToListAsync();

            var request = _http.HttpContext.Request;

            var result = hvs.Select(hv =>
            {
                gvDict.TryGetValue(hv.MaGv, out var gv);

                return new HocVienChuaPhanKhoaDTO
                {
                    HocVien = hv,
                    TenGv = gv?.TenGv,
                    HoTenDem = gv?.HoTenDem,
                    ImageUrl = string.IsNullOrEmpty(hv.DuongDanAnh)
                        ? null: $"{Constants.ApiPublicImage}?path={Uri.EscapeDataString(hv.DuongDanAnh)}"
                };//$"{baseUrl}/api//api/HocVienChuaPhanKhoa/image/{hv.IdHs}" đây là gọi api để load Image lên
            }).ToList();

            return result;
        }

 

    public async Task<(byte[] Bytes, string ContentType)?> GetImageByPathAsync(string uncPath)
    {
        if (string.IsNullOrWhiteSpace(uncPath))
            return null;

        var ts = await _gplx.QthtThamSoHts
            .FirstOrDefaultAsync(x => x.TenTs == "IMG_PATH_CSDT");

        if (ts == null) return null;

        var resolver = new ImagePathResolver(ts.GiaTriTs);

        var localPath = uncPath.Replace(resolver.UncRoot, resolver.LocalRoot);

        if (!File.Exists(localPath))
            return null;

        var ext = Path.GetExtension(localPath).ToLower();

        // ĐỌC FILE
        var bytes = await File.ReadAllBytesAsync(localPath);
        var contentType = "application/octet-stream";

        switch (ext)
        {
            case ".jpg":
            case ".jpeg":
                contentType = "image/jpeg";
                break;

            case ".png":
                contentType = "image/png";
                break;

            case ".jp2":
                // 🔥 Convert JP2 -> JPG để browser xem được
                using (var image = new MagickImage(bytes))
                {
                    image.Format = MagickFormat.Jpeg;
                    bytes = image.ToByteArray();
                }
                contentType = "image/jpeg";
                break;
        }

        return (bytes, contentType);
    }

    public async Task CreateAsync(HocVienChuaPhanKhoaRequest rq)
        {

            var model = new HocVienChuaPhanKhoa();
            rq.Patch(model);
            var file = rq.File;
            model.NgayNopHoSo = DateTime.Now;

            _context.HocVienChuaPhanKhoas.Add(model);
            await _context.SaveChangesAsync();

            var thamSoHt = await _gplx.QthtThamSoHts
                .FirstOrDefaultAsync(x => x.TenTs == "IMG_PATH_CSDT");

            var image_path = thamSoHt?.GiaTriTs ?? @"\\192.168.100.248\d\2026\im_gplx";

            var resolver = new ImagePathResolver(image_path);
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
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerMessage = ex.InnerException?.Message;

                // log ra console / file / serilog tùy hệ thống bạn đang dùng
                Console.WriteLine("=== SAVE CHANGES ERROR ===");
                Console.WriteLine(errorMessage);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("=== INNER ERROR ===");
                    Console.WriteLine(innerMessage);
                }

                throw new BadRequestException(
                    $"Lỗi khi lưu: {errorMessage} - {innerMessage}"
                );
            }

        }

        public async Task<bool> UpdateAsync(HocVienChuaPhanKhoaRequest rq)
        {
            var entity = await _context.HocVienChuaPhanKhoas
                .FirstOrDefaultAsync(x => x.IdHs == rq.IdHs);

            if (entity == null)
                return false;

            var oldImage = entity.DuongDanAnh;

            // update data
            rq.Patch(entity);

            // nếu request không gửi ảnh thì giữ ảnh cũ
            entity.DuongDanAnh = oldImage;

            var file = rq.File;

            // ===== upload ảnh mới =====
            if (file != null && file.Length > 0)
            {
                var thamSoHt = await _gplx.QthtThamSoHts
                    .FirstOrDefaultAsync(x => x.TenTs == "IMG_PATH_CSDT");

                var image_path = thamSoHt?.GiaTriTs
                    ?? @"\\192.168.100.248\d\2026\im_gplx";

                var resolver = new ImagePathResolver(image_path);

                var year = DateTime.Now.Year.ToString();

                var localFolder = Path.Combine(
                    resolver.LocalRoot,
                    year,
                    resolver.BaseFolder,
                    entity.SoCmt
                );

                Directory.CreateDirectory(localFolder);

                var ext = Path.GetExtension(file.FileName);

                var fileName =
                    $"{entity.SoCmt}-{DateTime.Now:yyyyMMdd-HHmmss}{ext}";

                var localFullPath = Path.Combine(localFolder, fileName);

                using (var stream = new FileStream(localFullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // update UNC path mới
                entity.DuongDanAnh = Path.Combine(
                    resolver.UncRoot,
                    year,
                    resolver.BaseFolder,
                    entity.SoCmt,
                    fileName
                );
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerMessage = ex.InnerException?.Message;

                Console.WriteLine("=== UPDATE ERROR ===");
                Console.WriteLine(errorMessage);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("=== INNER ERROR ===");
                    Console.WriteLine(innerMessage);
                }

                throw new BadRequestException(
                    $"Lỗi khi cập nhật: {errorMessage} - {innerMessage}"
                );
            }

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

        public async Task UpdateTrangThai(int IdHs)
        {
            var hv = await _context.HocVienChuaPhanKhoas.FindAsync(IdHs) ?? throw new BadRequestException("Không có Idhs này trong hệ thống");
            hv.TrangThai = !hv.TrangThai;
            await _context.SaveChangesAsync();
        }

        public async Task ChuyenLop(NguoiLxCreateRequest request)
        {
            var hv = await _context.HocVienChuaPhanKhoas.Where(x => x.SoCmt == request.SoCmt && x.HangDaoTao == request.HangDaoTao).FirstOrDefaultAsync()
                ?? throw new BadRequestException("Không có Idhs này trong hệ thống");
            var madk = await _nguoiLxes.CreateAsync(request);    
            
            hv.MaDk = madk;
            hv.TrangThai = false;
            await _context.SaveChangesAsync();
        }
    }
}
