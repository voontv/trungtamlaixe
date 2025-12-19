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
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using System.Collections.Generic;
using Ttlaixe.DTO.request.Ttlaixe.DTO.request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Xml;
using Microsoft.AspNetCore.Server.IIS.Core;
namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(NguoiLxesBusinesses))]
    public interface INguoiLxesBusinesses
    {
        Task<NguoiLxResponse> CreateAsync(NguoiLxCreateRequest request);

        Task<bool> UpdateAsync(NguoiLxResponse rq);

        Task<List<NguoiLxCoBanResponse>> GetThongTinCoBanByKhoaHocAsync(string maKhoaHoc);

        Task<List<NguoiLxThiResponse>> GetDanhSachSatHach(string MaSatHach);

        Task<NguoiLxResponse> GetThongTinNguoiLx(string maDk);

        Task UpdateHinhThe(IFormFile file, string maDk);

        Task UpdateMaBcByMaDksAsync(IFormFile file);
    }

    public class NguoiLxesBusinesses : ControllerBase, INguoiLxesBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenInfo _authenInfo;
        private readonly UploadOptions _opt;
        public NguoiLxesBusinesses(GplxCsdtContext context, ITokenGenerator tokenGenerator, IAuthenInfo authenInfo, IOptions<UploadOptions> opt)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _authenInfo = authenInfo;
            _opt = opt.Value;
        }

        public async Task<NguoiLxResponse> CreateAsync(NguoiLxCreateRequest rq)
        {
            var file = rq.File;
            var now = DateTime.Now;
            var logged = _authenInfo.Get();
            var actor = await _context.UserTkns.FindAsync(logged.UserName);
            if (!actor.QuyenAdmin && !actor.QuyenNhapLieu)
            {
                throw new BadRequestException("Bạn không có quyền thực hiện tính năng này. ");
            }

            if (await ExistsSoCmtInKhoaHocAsync(rq.MaCsdt, rq.MaKhoaHoc, rq.SoCmt))
            {
                throw new BadRequestException("Số CMT này đã tồn tại trong khóa học này.");
            }
            // ============= 0. Sinh MaDK = <MaCSDT>-<yyyyMMddHHmmssfff> =============
            // Ví dụ: 48012-20250419150455500
            var maDk = $"{Constants.MaCSDT}-{now:yyyyMMddHHmmssfff}";

            // ============= 1. Tạo NguoiLx =============
            var nguoi = new NguoiLx();
            rq.Patch(nguoi);
            nguoi.MaDk = maDk;                 // set lại sau Patch để không bị đè null
            nguoi.DonViNhanHso = rq.MaCsdt;
            nguoi.HoVaTen = $"{nguoi.HoDemNlx} {nguoi.TenNlx}";
            nguoi.HoVaTenIn = nguoi.HoVaTen;
            nguoi.NoiCt = "";
            nguoi.NoiTt = "";

            // ============= 2. Sinh SoHoSo (001, 002, ..., 029, ...) =============
            var lastSoHoSo = await _context.NguoiLxHoSos
                .Where(x => x.MaCsdt == Constants.MaCSDT && x.MaKhoaHoc == rq.MaKhoaHoc)
                .OrderByDescending(x => x.SoHoSo)
                .Select(x => x.SoHoSo)
                .FirstOrDefaultAsync();

            int nextSoHoSoNumber = 1;
            if (!string.IsNullOrEmpty(lastSoHoSo) && int.TryParse(lastSoHoSo, out var parsed))
            {
                nextSoHoSoNumber = parsed + 1;
            }

            var soHoSo = nextSoHoSoNumber.ToString("D3"); // ví dụ "029"

            // ============= 3. Tạo NguoiLxHoSo =============
            // ===== Update NguoiLxHoSo =====
            var maLoaiHs = 2;
            string[] motoCodes = { "A01", "A02", "A1", "A1m", "A2" };

            if (motoCodes.Contains(rq.HangGplx))
            {
                maLoaiHs = 1;
            }

            var maHeThongCap = "CM_VN";
            if (nguoi.MaQuocTich != "VNM")
            {
                maHeThongCap = "CM_EN";
            }
            var hoSo = new NguoiLxHoSo
            {
                MaCsdt = Constants.MaCSDT,
                MaSoGtvt = Constants.MaSoGTVT,
                MaDvnhanHso = Constants.MaCSDT,
                NgayNhanHso = now,
                MaLoaiHs = maLoaiHs,
                TtXuLy = "01",
                ChonInGplx = 2,
                GiayCnsk = false,
                TransferFlag = 0,
                HosoDvcc4 = 0,
                TrangThai = true,
                MaHtcap = maHeThongCap,
                NgayTao = now,
                NgaySua = now
            };

            rq.Patch(hoSo);
            hoSo.MaDk = maDk;                  // <<< BẮT BUỘC
            hoSo.SoHoSo = soHoSo;              // <<< nên set luôn

            if (rq.GiayTos != null && rq.GiayTos.Count > 0)
            {
                foreach (var gt in rq.GiayTos)
                {
                    var gtHoSo = new NguoiLxhsGiayTo
                    {
                        MaGt = gt.MaGt,
                        MaDk = maDk,
                        SoHoSo = soHoSo,
                        TenGt = gt.TenGt,
                        TrangThai = true
                    };

                    _context.NguoiLxhsGiayTos.Add(gtHoSo);
                }


            }

            // ============= 5. Lưu tất cả vào DB =============
            _context.NguoiLxes.Add(nguoi);
            _context.NguoiLxHoSos.Add(hoSo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                var detail = baseEx?.Message ?? ex.InnerException?.Message ?? ex.Message;

                throw new BadRequestException("Error found is 1111 " + detail);
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Error found is 22222" + ex.Message);
            }


            var nguoiLaiXeRes = new NguoiLxResponse();
            rq.Patch(nguoiLaiXeRes);
            nguoiLaiXeRes.MaDk = maDk;
            if (file != null && file.Length > 0)
            {
                var savedPath = await SaveToRelativePathAsync(file, hoSo.MaDk);

                // Update đường dẫn ảnh vào hồ sơ
                hoSo.DuongDanAnh = savedPath.Replace(_opt.ImageRoot,_opt.ImageSaveDatabase);

                // Nếu bạn muốn TT_XuLy đổi theo có ảnh:
                hoSo.TtXuLy = "03";

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    var baseEx = ex.GetBaseException(); // thường là SqlException
                    var detail = baseEx?.Message ?? ex.Message;

                    // Nếu muốn xem thêm stack (dev thôi):
                    // detail += "\n" + ex.ToString();

                    throw new BadRequestException("Error found is 3333" + detail);
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("Error found is 4444" + ex.Message);
                }
            }

            return nguoiLaiXeRes;
        }

        public async Task<bool> UpdateAsync(NguoiLxResponse rq)
        {
            var file = rq.File;
            var now = DateTime.Now;
            var logged = _authenInfo.Get();
            var actor = await _context.UserTkns.FindAsync(logged.UserName);
            if (!actor.QuyenAdmin && !actor.QuyenNhapLieu)
            {
                throw new BadRequestException("Bạn không có quyền thực hiện tính năng này. ");
            }

            if (string.IsNullOrWhiteSpace(rq.MaDk))
                throw new Exception("MaDk không được để trống khi cập nhật");

            var maDk = rq.MaDk;

            var nguoi = await _context.NguoiLxes
                .FirstOrDefaultAsync(x => x.MaDk == maDk);
            if (nguoi == null)
                throw new Exception($"Không tìm thấy người lái với MaDK = {maDk}");

            var hoSo = await _context.NguoiLxHoSos
                .FirstOrDefaultAsync(x => x.MaDk == maDk);
            if (hoSo == null)
                throw new Exception($"Không tìm thấy hồ sơ với MaDK = {maDk}");
            rq.Patch(hoSo);

            var blockStatuses = new[] { "05", "11", "12", "13" };
            if (!string.IsNullOrEmpty(hoSo.TtXuLy) && blockStatuses.Contains(hoSo.TtXuLy))
                throw new Exception($"Không được phép chỉnh sửa hồ sơ khi TT_XuLy = {hoSo.TtXuLy}");
            var maLoaiHs = 2;
            string[] motoCodes = { "A01", "A02", "A1", "A1m", "A2" };

            if (motoCodes.Contains(rq.HangGplx))
            {
                maLoaiHs = 1;
            }
            rq.Patch(nguoi);
            nguoi.TrangThai = nguoi.TrangThai ?? true;
            nguoi.NgaySua = now;
            nguoi.MaQuocTich = string.IsNullOrWhiteSpace(rq.MaQuocTich) ? "VNM" : rq.MaQuocTich;


            hoSo.MaLoaiHs = maLoaiHs;

            hoSo.NgaySua = now;

            var soHoSo = hoSo.SoHoSo;

            // ===== Reset & add lại Giấy tờ =====
            var oldGiayTos = await _context.NguoiLxhsGiayTos
                .Where(x => x.MaDk == maDk && x.SoHoSo == soHoSo)
                .ToListAsync();

            if (oldGiayTos.Any())
                _context.NguoiLxhsGiayTos.RemoveRange(oldGiayTos);

            if (rq.GiayTos != null && rq.GiayTos.Count > 0)
            {
                foreach (var gt in rq.GiayTos)
                {
                    _context.NguoiLxhsGiayTos.Add(new NguoiLxhsGiayTo
                    {
                        MaGt = gt.MaGt,
                        MaDk = maDk,
                        SoHoSo = soHoSo,
                        TenGt = gt.TenGt,
                        TrangThai = true
                    });
                }
            }
            if (file != null && file.Length > 0)
            {
                var savedPath = await SaveToRelativePathAsync(file, maDk);
                hoSo.DuongDanAnh = savedPath;
                if (string.IsNullOrEmpty(hoSo.TtXuLy) || hoSo.TtXuLy == "01")
                    hoSo.TtXuLy = "03";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsSoCmtInKhoaHocAsync(string maCSDT, string maKhoaHoc, string soCmt)
        {
            return await (
                from h in _context.NguoiLxHoSos
                join n in _context.NguoiLxes
                    on h.MaDk equals n.MaDk
                where h.MaCsdt == maCSDT
                      && h.MaKhoaHoc == maKhoaHoc
                      && n.SoCmt == soCmt
                select n
            ).AnyAsync();
        }

        public async Task<NguoiLxResponse> GetThongTinNguoiLx(string maDk)
        {
            var nguoiLxHoSo = await _context.NguoiLxHoSos.FindAsync(maDk);
            var nguoiLx = await _context.NguoiLxes.FindAsync(maDk);
            var nguoiLxGiayTo = await _context.NguoiLxhsGiayTos.Where(x => x.MaDk == maDk).ToListAsync();

            var nguoiLxRes = new NguoiLxResponse();
            nguoiLxHoSo.Patch(nguoiLxRes);
            nguoiLx.Patch(nguoiLxRes);
            nguoiLxGiayTo.Patch(nguoiLxRes.GiayTos);
            return nguoiLxRes;
        }

        public async Task<List<NguoiLxCoBanResponse>> GetThongTinCoBanByKhoaHocAsync(string maKhoaHoc)
        {
            var query =
                from h in _context.NguoiLxHoSos
                join n in _context.NguoiLxes
                    on h.MaDk equals n.MaDk
                join dvNoiTT in _context.DmDvhcs
                    on n.NoiTtMaDvhc equals dvNoiTT.MaDvhc into dvNoiTTJoin
                from dvNoiTT in dvNoiTTJoin.DefaultIfEmpty()
                join dvNoiCT in _context.DmDvhcs
                    on n.NoiCtMaDvhc equals dvNoiCT.MaDvhc into dvNoiCTJoin
                from dvNoiCT in dvNoiCTJoin.DefaultIfEmpty()
                where h.MaCsdt == Constants.MaCSDT
                      && h.MaKhoaHoc == maKhoaHoc
                select new NguoiLxCoBanResponse
                {
                    MaDk = h.MaDk,
                    MaKhoaHoc = h.MaKhoaHoc,
                    MaCsdt = h.MaCsdt,
                    HoVaTen = n.HoVaTen,
                    SoCmt = n.SoCmt,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh,
                    NoiThuongTru =
                        (dvNoiTT != null ? dvNoiTT.TenDayDu : ""),
                    NoiCuTru =
                        (dvNoiCT != null ? dvNoiCT.TenDayDu : ""),
                    NgayNhanHso = h.NgayNhanHso
                };

            // GỘP LẠI MỖI NGƯỜI 1 DÒNG
            var result = await query
                .GroupBy(x => new
                {
                    x.MaDk,
                    x.MaKhoaHoc,
                    x.MaCsdt,
                    x.HoVaTen,
                    x.SoCmt,
                    x.NgaySinh,
                    x.GioiTinh
                })
                .Select(g => new NguoiLxCoBanResponse
                {
                    MaDk = g.Key.MaDk,
                    MaKhoaHoc = g.Key.MaKhoaHoc,
                    MaCsdt = g.Key.MaCsdt,
                    HoVaTen = g.Key.HoVaTen,
                    SoCmt = g.Key.SoCmt,
                    NgaySinh = g.Key.NgaySinh,
                    GioiTinh = g.Key.GioiTinh,
                    NoiThuongTru = g.Select(x => x.NoiThuongTru).FirstOrDefault(),
                    NoiCuTru = g.Select(x => x.NoiCuTru).FirstOrDefault(),
                    NgayNhanHso = g.Select(x => x.NgayNhanHso).Min() // hoặc FirstOrDefault
                })
                .ToListAsync();

            return result;
        }

        /*
         * 
         * 
         * 
         * public async Task<bool> LuuKetQuaChiTietPhanThiAsync(
    List<ThiSatHachKetQuaChiTietTknRequest> items)
        {
            if (items == null || items.Count == 0)
                return true;

            // Giả sử list cùng 1 MaKySh, MaDk, MaPhanThi
            var maKySh = items[0].MaKySh;
            var maDk = items[0].MaDk;
            var maPhanThi = items[0].MaPhanThi;

            // Lấy các dòng hiện có trong DB cho thí sinh + bài thi này
            var existing = await _context.ThiSatHachKetQuaChiTietTkns
                .Where(x => x.MaKySh == maKySh
                            && x.MaDk == maDk
                            && x.MaPhanThi == maPhanThi)
                .ToListAsync();

            // Map cho nhanh
            var existingDict = existing.ToDictionary(
                x => x.IdQuyTac,
                x => x
            );

            var idQuyTacTrongRequest = items.Select(x => x.IdQuyTac).ToHashSet();

            // 1. Xử lý từng dòng request
            foreach (var rq in items)
            {
                existingDict.TryGetValue(rq.IdQuyTac, out var entity);

                if (rq.SoLanPham <= 0)
                {
                    // Nếu số lần phạm <= 0 -> xóa nếu đang có trong DB
                    if (entity != null)
                    {
                        _context.ThiSatHachKetQuaChiTietTkns.Remove(entity);
                    }
                }
                else
                {
                    // SoLanPham > 0 -> insert hoặc update
                    if (entity == null)
                    {
                        entity = new ThiSatHachKetQuaChiTietTkn
                        {
                            MaKySh = rq.MaKySh,
                            MaDk = rq.MaDk,
                            MaPhanThi = rq.MaPhanThi,
                            IdQuyTac = rq.IdQuyTac,
                            SoLanPham = rq.SoLanPham
                        };
                        _context.ThiSatHachKetQuaChiTietTkns.Add(entity);
                    }
                    else
                    {
                        entity.SoLanPham = rq.SoLanPham;
                    }
                }
            }

            // 2. Xóa các lỗi cũ không còn trong request nữa (nếu muốn “clean” luôn)
            var toRemove = existing
                .Where(x => !idQuyTacTrongRequest.Contains(x.IdQuyTac))
                .ToList();

            foreach (var e in toRemove)
            {
                _context.ThiSatHachKetQuaChiTietTkns.Remove(e);
            }

            await _context.SaveChangesAsync();

            return true;
        }*/

        public async Task<List<string>> GetMaShatHach()
        {
            var result = await _context.NguoiLxHoSos
                    .Where(x => !string.IsNullOrEmpty(x.MaKySh))
                    .GroupBy(x => x.MaKySh)
                    // chỉ giữ những MaKySh mà KHÔNG có bất kỳ dòng nào KetQuaSh != null
                    .Where(g => !g.Any(r => r.KetQuaSh != null))   // KetQuaSh: đổi theo đúng tên property
                    .Select(g => g.Key)
                    .ToListAsync();

            return result;
        }

        public async Task<List<NguoiLxThiResponse>> GetDanhSachSatHach(string MaSatHach)
        {
            // 1. Chuẩn hóa DM_DVHC: mỗi MaDvhc chỉ còn 1 dòng
            var dvhcChuan =
                from dv in _context.DmDvhcs.AsNoTracking()
                group dv by dv.MaDvhc into g
                select new
                {
                    MaDvhc = g.Key,
                    TenDayDu = g.Min(x => x.TenDayDu)  // hoặc Max, hoặc Min theo Id... tùy bạn
                };

            // 2. Lọc trước trên bảng hồ sơ
            var coTheThiHoSos =
                from h in _context.NguoiLxHoSos.AsNoTracking()
                where h.MaKySh == MaSatHach
                select new
                {
                    h.MaDk,
                    h.MaKhoaHoc,
                    h.MaCsdt,
                    h.NgayNhanHso
                };

            // 3. Join với các bảng khác + DM_DVHC chuẩn
            var query =
                from h in coTheThiHoSos
                join n in _context.NguoiLxes.AsNoTracking()
                    on h.MaDk equals n.MaDk
                join kh in _context.KhoaHocs.AsNoTracking()
                    on h.MaKhoaHoc equals kh.MaKh
                // nơi thường trú
                join dvNoiTT in dvhcChuan
                    on n.NoiTtMaDvhc equals dvNoiTT.MaDvhc into dvNoiTTJoin
                from dvNoiTT in dvNoiTTJoin.DefaultIfEmpty()
                    // nơi cư trú
                join dvNoiCT in dvhcChuan
                    on n.NoiCtMaDvhc equals dvNoiCT.MaDvhc into dvNoiCTJoin
                from dvNoiCT in dvNoiCTJoin.DefaultIfEmpty()
                select new NguoiLxThiResponse
                {
                    MaDk = h.MaDk,
                    MaKhoaHoc = h.MaKhoaHoc,
                    MaCsdt = h.MaCsdt,
                    HoVaTen = n.HoVaTen,
                    SoCmt = n.SoCmt,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh,
                    NoiThuongTru = dvNoiTT != null ? dvNoiTT.TenDayDu : "",
                    NoiCuTru = dvNoiCT != null ? dvNoiCT.TenDayDu : "",
                    NgayNhanHso = h.NgayNhanHso,
                    TenKh = kh.TenKh
                };

            // 4. Không cần GroupBy nữa, vì mỗi MaDvhc đã chỉ còn 1 bản ghi
            var result = await query.ToListAsync();

            return result;
        }

        

        public async Task UpdateHinhThe(IFormFile file, string maDk)
        {
            var logged = _authenInfo.Get();
            var actor = await _context.UserTkns.FindAsync(logged.UserName);
            if (!actor.QuyenAdmin && !actor.QuyenNhapLieu)
            {
                throw new BadRequestException("Bạn không có quyền thực hiện tính năng này. ");
            }

            var hoSo = await _context.NguoiLxHoSos.FindAsync(maDk) ?? throw new BadRequestException("Không tìm thấy thông tin người học lái xe.");
            var savedPath = await SaveToRelativePathAsync(file, hoSo.MaDk);

            // Update đường dẫn ảnh vào hồ sơ
            hoSo.DuongDanAnh = savedPath.Replace(_opt.ImageRoot, _opt.ImageSaveDatabase);

            // Nếu bạn muốn TT_XuLy đổi theo có ảnh:
            hoSo.TtXuLy = "03";

            await _context.SaveChangesAsync();
        }
        private async Task<string> SaveToRelativePathAsync(IFormFile file, string maDk)
        {
            var nguoiLx = await _context.NguoiLxHoSos.FindAsync(maDk)
        ?? throw new BadRequestException("Không có thông tin người này");

            // Lấy đuôi từ file upload (".png", ".jpg", ".jp2"...)
            var ext = Path.GetExtension(file.FileName);

            // fallback nếu không có ext
            if (string.IsNullOrWhiteSpace(ext))
                ext = Utils.GetExtFromContentType(file.ContentType) ?? ".bin";

            // normalize ext
            if (!ext.StartsWith(".")) ext = "." + ext;

            // relativePath đầy đủ gồm cả tên file
            var relativePath = Path.Combine(nguoiLx.MaKhoaHoc, maDk + ext);

            return await Utils.SaveToRelativePathAsync(file, relativePath, _opt.ImageRoot);
        }

        public async Task UpdateMaBcByMaDksAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("Vui lòng chọn file XML.");

            var ext = Path.GetExtension(file.FileName);
            if (!string.Equals(ext, ".xml", StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Chỉ cho phép upload file .xml.");

            DanhSachBc1 group;
            await using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;

                try
                {
                    group = Utils.DeserializeBaoCao1(ms);
                }
                catch (InvalidOperationException ex)
                {
                    throw new BadRequestException($"XML không đúng schema / không đọc được. Chi tiết: {ex.Message}");
                }
                catch (XmlException ex)
                {
                    throw new BadRequestException($"XML bị lỗi cú pháp. Chi tiết: {ex.Message}");
                }
            }

            if (string.IsNullOrWhiteSpace(group.MaBci))
                throw new BadRequestException("MA_BCI không tồn tại trong XML.");

            if (group.MaDks == null || group.MaDks.Count == 0)
                throw new BadRequestException("Không có MA_DK nào trong XML.");

            if (group == null) throw new ArgumentNullException(nameof(group));
            if (string.IsNullOrWhiteSpace(group.MaBci)) throw new BadRequestException("MaBci is required.");
            if (group.MaDks == null || group.MaDks.Count == 0) throw new BadRequestException("Không có học viên nào trong xml.");

            // Chuẩn hoá + loại trùng + bỏ rỗng
            var maDks = group.MaDks
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (maDks.Count == 0)
                throw new BadRequestException("Danh sách MaDks rỗng sau khi chuẩn hoá.");

            // ========== 1) VALIDATE TRƯỚC (gặp lỗi -> throw) ==========
            // để tránh query IN quá dài, chia chunk
            const int chunkSize = 500;
            var existed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < maDks.Count; i += chunkSize)
            {
                var chunk = maDks.Skip(i).Take(chunkSize).ToList();

                var found = await _context.NguoiLxHoSos
                    .Where(x => chunk.Contains(x.MaDk))
                    .Select(x => x.MaDk)
                    .ToListAsync();

                foreach (var f in found) existed.Add(f);
            }

            var notFound = maDks.Where(x => !existed.Contains(x)).ToList();
            if (notFound.Count > 0)
            {
                // tránh trả message quá dài
                var preview = string.Join(", ", notFound.Take(50));
                var suffix = notFound.Count > 50 ? $" ... (+{notFound.Count - 50} mã khác)" : "";
                throw new BadRequestException($"Có {notFound.Count} mã MA_DK không tồn tại: {preview}{suffix}");
            }

            // ========== 2) UPDATE BULK (nhanh) ==========
            // EF Core 7+:
            for (int i = 0; i < maDks.Count; i += chunkSize)
            {
                var chunk = maDks.Skip(i).Take(chunkSize).ToList();

                await _context.NguoiLxHoSos
                    .Where(x => chunk.Contains(x.MaDk))
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.MaBc1, group.MaBci)
                    );
            }
        }


    }
}
