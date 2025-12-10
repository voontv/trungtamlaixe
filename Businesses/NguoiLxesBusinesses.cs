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
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using System.Collections.Generic;
using Ttlaixe.DTO.request.Ttlaixe.DTO.request;
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
    }

    public class NguoiLxesBusinesses : ControllerBase, INguoiLxesBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenInfo _authenInfo;
        public NguoiLxesBusinesses(GplxCsdtContext context, ITokenGenerator tokenGenerator, IAuthenInfo authenInfo)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _authenInfo = authenInfo;
        }

        public async Task<NguoiLxResponse> CreateAsync(NguoiLxCreateRequest rq)
        {
            var now = DateTime.Now;
            //var user = _authenInfo.Get();

            if (await ExistsSoCmtInKhoaHocAsync(rq.MaCsdt, rq.MaKhoaHoc, rq.SoCmt))
            {
                throw new BadRequestException("Số CMT này đã tồn tại trong khóa học này.");
            }
            // ============= 0. Sinh MaDK = <MaCSDT>-<yyyyMMddHHmmssfff> =============
            // Ví dụ: 48012-20250419150455500
            var maDk = $"{Constants.MaCSDT}-{now:yyyyMMddHHmmssfff}";

            // ============= 1. Tạo NguoiLx =============
            var nguoi = new NguoiLx
            {
                MaDk = maDk,
                DonViNhanHso = rq.MaCsdt,
                HoDemNlx = rq.HoDemNlx?.Trim(),
                TenNlx = rq.TenNlx?.Trim()
            };

            nguoi.HoVaTen = $"{nguoi.HoDemNlx} {nguoi.TenNlx}".Trim();
            nguoi.HoVaTenIn = nguoi.HoVaTen.ToUpper();

            nguoi.MaQuocTich = string.IsNullOrWhiteSpace(rq.MaQuocTich)
                ? "VNM"
                : rq.MaQuocTich;
            nguoi.NgaySinh = rq.NgaySinh;   // "YYYYMMDD"
            

            // Thường trú
            nguoi.NoiTt = rq.NoiTt;
            nguoi.NoiTtMaDvhc = rq.NoiTtMaDvhc;
            nguoi.NoiTtMaDvql = rq.NoiTtMaDvql;

            nguoi.NoiCtMaDvhc = rq.NoiCtMaDvhc;
            nguoi.NoiCtMaDvql = rq.NoiCtMaDvql;

            // CMT/CCCD
            nguoi.SoCmt = rq.SoCmt;
            nguoi.NgayCapCmt = rq.NgayCapCmt;
            nguoi.NoiCapCmt = rq.NoiCapCmt;

            nguoi.GhiChu = rq.GhiChu;
            nguoi.GioiTinh = rq.GioiTinh;
            nguoi.SoCmndCu = rq.SoCmndCu;

            // Metadata
            nguoi.TrangThai = true;
            //nguoi.NguoiTao = user?.UserId;
            //nguoi.NguoiSua = user?.UserId;
            nguoi.NgayTao = now;
            nguoi.NgaySua = now;
            nguoi.HosoDvcc4 = 0;
            var ttxuly = string.IsNullOrEmpty(rq.DuongDanAnh) ? "01" : "03";
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
            if (rq.HangGplx == "A.01"
    || rq.HangGplx == "A.02"
    || rq.HangGplx == "A.03"
    || rq.HangGplx == "A1m"
    || rq.HangGplx == "Am")
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
                MaDk = maDk,
                SoHoSo = soHoSo,
                MaCsdt = Constants.MaCSDT,
                MaSoGtvt = Constants.MaSoGTVT,
                MaDvnhanHso = Constants.MaCSDT,

                NgayNhanHso = now,
                MaLoaiHs = maLoaiHs,

                TtXuLy = ttxuly,

            DonViHocLx = rq.MaCsdt,
                NamHocLx = rq.NamHocLx,
                HangGplx = rq.HangGplx,     // hạng đề nghị cấp
                HangDaoTao = rq.HangDaoTao,   // hạng đào tạo
                MaKhoaHoc = rq.MaKhoaHoc,

                GiayCnsk = false,
                TransferFlag = 0,
                HosoDvcc4 = 0,
                TrangThai = true,
                MaHtcap = maHeThongCap,
                //NguoiTao = user?.UserId,
                //NguoiSua = user?.UserId,
                NgayTao = now,
                NgaySua = now
            };

            // ============= 4. Tạo list giấy tờ NguoiLxhsGiayTo từ request =============
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
            catch(Exception ex)
            {
                throw new BadRequestException("Error found is " + ex.Message);
            }
            
            var nguoiLaiXeRes = new NguoiLxResponse();
            rq.Patch(nguoiLaiXeRes);
            nguoiLaiXeRes.MaDk = maDk;

            return nguoiLaiXeRes;
        }

        public async Task<bool> UpdateAsync(NguoiLxResponse rq)
        {
            var now = DateTime.Now;
            var user = _authenInfo.Get();

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

            var blockStatuses = new[] { "05", "11", "12", "13" };
            if (!string.IsNullOrEmpty(hoSo.TtXuLy) && blockStatuses.Contains(hoSo.TtXuLy))
                throw new Exception($"Không được phép chỉnh sửa hồ sơ khi TT_XuLy = {hoSo.TtXuLy}");

            // ===== Update NguoiLx =====
            nguoi.HoDemNlx = rq.HoDemNlx?.Trim();
            nguoi.TenNlx = rq.TenNlx?.Trim();
            nguoi.HoVaTen = $"{nguoi.HoDemNlx} {nguoi.TenNlx}".Trim();
            nguoi.HoVaTenIn = nguoi.HoVaTen.ToUpper();

            nguoi.MaQuocTich = string.IsNullOrWhiteSpace(rq.MaQuocTich) ? "VNM" : rq.MaQuocTich;
            nguoi.NgaySinh = rq.NgaySinh;
            nguoi.NoiTt = rq.NoiTt;
            nguoi.NoiTtMaDvhc = rq.NoiTtMaDvhc;
            nguoi.NoiTtMaDvql = rq.NoiTtMaDvql;
            nguoi.NoiCtMaDvhc = rq.NoiCtMaDvhc;
            nguoi.NoiCtMaDvql = rq.NoiCtMaDvql;
            nguoi.SoCmt = rq.SoCmt;
            nguoi.NgayCapCmt = rq.NgayCapCmt;
            nguoi.NoiCapCmt = rq.NoiCapCmt;
            nguoi.GhiChu = rq.GhiChu;
            nguoi.GioiTinh = rq.GioiTinh;
            nguoi.SoCmndCu = rq.SoCmndCu;
            nguoi.TrangThai = nguoi.TrangThai ?? true;
            nguoi.NgaySua = now;

           

            hoSo.MaLoaiHs = rq.MaLoaiHs;
            hoSo.MaCsdt = rq.MaCsdt;
            hoSo.NamHocLx = rq.NamHocLx;
            hoSo.HangGplx = rq.HangGplx;
            hoSo.HangDaoTao = rq.HangDaoTao;
            hoSo.MaKhoaHoc = rq.MaKhoaHoc;
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

        public async Task<bool> LuuKetQuaChiTietPhanThiAsync(
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
        }

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

        public async Task<ThiSatHachKetQuaPhanThiTkn> TinhVaLuuKetQuaPhanThiAsync(TinhKetQuaPhanThiRequest rq)
        {
            // 1. Lấy danh sách quy tắc + ghép chi tiết lỗi của thí sinh
            var query =
                from q in _context.DmthiSatHachQuyTacTkns.AsNoTracking()
                join p in _context.DmPhanThiTkns.AsNoTracking()
                    on q.MaPhanThi equals p.MaPhanThi
                join ct in _context.ThiSatHachKetQuaChiTietTkns.AsNoTracking()
                    .Where(x => x.MaKySh == rq.MaKySh
                                && x.MaDk == rq.MaDk
                                && x.MaPhanThi == rq.MaPhanThi)
                    on q.IdQuyTac equals ct.IdQuyTac into ctJoin
                from ct in ctJoin.DefaultIfEmpty()
                where q.MaPhanThi == rq.MaPhanThi
                      && p.HangDaoTao == rq.HangDaoTao
                select new
                {
                    QuyTac = q,
                    SoLanPham = ct != null ? ct.SoLanPham : 0
                };

            var list = await query.ToListAsync();

            // 2. Nếu không có quy tắc nào -> mặc định Đạt, 0 điểm trừ
            if (list.Count == 0)
            {
                var upsertRq = new ThiSatHachKetQuaPhanThiTknUpdate
                {
                    MaKySh = rq.MaKySh,
                    MaDk = rq.MaDk,
                    MaPhanThi = rq.MaPhanThi,
                    HangDaoTao = rq.HangDaoTao,
                    MaNguoiCham = rq.MaNguoiCham,
                    DiemToiDa = rq.DiemToiDa,
                    TongDiemTru = 0,
                    DiemConLai = rq.DiemToiDa,
                    KetQua = true,
                    GhiChu = null
                };

                return await UpsertKetQuaPhanThiAsync(upsertRq);
            }

            // 3. Check lỗi "rớt ngay"
            bool hasRoiNgay = list.Any(x =>
                x.QuyTac.IsRotNgay == true &&
                x.SoLanPham > 0
            );

            // 4. Tính tổng điểm trừ
            int tongDiemTru = list
                .Where(x =>
                    !x.QuyTac.IsRotNgay &&
                    x.SoLanPham > 0 &&
                    x.QuyTac.DonViDiemTru > 0)
                .Sum(x => x.SoLanPham * x.QuyTac.DonViDiemTru);

            int diemConLai = rq.DiemToiDa - tongDiemTru;
            if (diemConLai < 0)
                diemConLai = 0;

            // 5. Kết quả
            bool ketQua = !hasRoiNgay;

            var request = new ThiSatHachKetQuaPhanThiTknUpdate
            {
                MaKySh = rq.MaKySh,
                MaDk = rq.MaDk,
                MaPhanThi = rq.MaPhanThi,
                HangDaoTao = rq.HangDaoTao,
                MaNguoiCham = rq.MaNguoiCham,
                DiemToiDa = rq.DiemToiDa,
                TongDiemTru = tongDiemTru,   // ✅ dùng đúng biến đã tính
                DiemConLai = diemConLai,
                KetQua = ketQua,
                GhiChu = null
            };

            var ketQuaEntity = await UpsertKetQuaPhanThiAsync(request);

            return ketQuaEntity;
        }

        private async Task<ThiSatHachKetQuaPhanThiTkn> UpsertKetQuaPhanThiAsync(ThiSatHachKetQuaPhanThiTknUpdate rq)
        {
            var existing = await _context.ThiSatHachKetQuaPhanThiTkns
                .FirstOrDefaultAsync(x =>
                    x.MaKySh == rq.MaKySh &&
                    x.MaDk == rq.MaDk &&
                    x.MaPhanThi == rq.MaPhanThi);

            if (existing == null)
            {
                existing = new ThiSatHachKetQuaPhanThiTkn
                {
                    MaKySh = rq.MaKySh,
                    MaDk = rq.MaDk,
                    MaPhanThi = rq.MaPhanThi,
                    HangDaoTao = rq.HangDaoTao,
                    MaNguoiCham = rq.MaNguoiCham,
                    DiemToiDa = rq.DiemToiDa,
                    TongDiemTru = rq.TongDiemTru,
                    DiemConLai = rq.DiemConLai,
                    KetQua = rq.KetQua,
                    GhiChu = rq.GhiChu
                };

                _context.ThiSatHachKetQuaPhanThiTkns.Add(existing);
            }
            else
            {
                existing.HangDaoTao = rq.HangDaoTao;
                existing.MaNguoiCham = rq.MaNguoiCham;
                existing.DiemToiDa = rq.DiemToiDa;
                existing.TongDiemTru = rq.TongDiemTru;
                existing.DiemConLai = rq.DiemConLai;
                existing.KetQua = rq.KetQua;
                // GhiChu tùy bạn có cho sửa theo rq.GhiChu hay không
            }

            await _context.SaveChangesAsync();
            return existing;
        }


    }
}
