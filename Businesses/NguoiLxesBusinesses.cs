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
namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(NguoiLxesBusinesses))]
    public interface INguoiLxesBusinesses
    {
        Task<NguoiLxResponse> CreateAsync(NguoiLxCreateRequest request);

        Task<bool> UpdateAsync(NguoiLxResponse rq);
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
            var user = _authenInfo.Get();

            // ============= 0. Sinh MaDK = <MaCSDT>-<yyyyMMddHHmmssfff> =============
            // Ví dụ: 48012-20250419150455500
            var maDk = $"{Constants.MaCSDT}-{now:yyyyMMddHHmmssfff}";

            // ============= 1. Tạo NguoiLx =============
            var nguoi = new NguoiLx
            {
                MaDk = maDk,
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

            // Cư trú
            nguoi.NoiCt = rq.NoiCt;
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

            // ============= 2. Sinh SoHoSo (001, 002, ..., 029, ...) =============
            var lastSoHoSo = await _context.NguoiLxHoSos
                .Where(x => x.MaCsdt == Constants.MaCSDT)
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
            var maLoaiHs = rq.MaLoaiHs.HasValue && rq.MaLoaiHs.Value > 0
                ? rq.MaLoaiHs.Value
                : 3;   // ví dụ: 3 = hồ sơ đào tạo mới

            var hoSo = new NguoiLxHoSo
            {
                MaDk = maDk,
                SoHoSo = soHoSo,
                MaCsdt = Constants.MaCSDT,
                MaSoGtvt = Constants.MaSoGTVT,
                MaDvnhanHso = Constants.MaCSDT,

                NgayNhanHso = now,
                MaLoaiHs = maLoaiHs,
                TtXuLy = "1",   // trạng thái xử lý ban đầu, tùy DM_TrangThai

                DonViHocLx = rq.DonViHocLx,
                NamHocLx = rq.NamHocLx,
                HangGplx = rq.HangGplx,     // hạng đề nghị cấp
                HangDaoTao = rq.HangDaoTao,   // hạng đào tạo
                MaKhoaHoc = rq.MaKhoaHoc,

                GiayCnsk = false,
                TransferFlag = 0,
                HosoDvcc4 = 0,
                TrangThai = true,

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

            await _context.SaveChangesAsync();
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
            nguoi.NoiCt = rq.NoiCt;
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

            // ===== Update NguoiLxHoSo =====
            var maLoaiHs = rq.MaLoaiHs.HasValue && rq.MaLoaiHs.Value > 0
                ? rq.MaLoaiHs.Value
                : hoSo.MaLoaiHs;

            hoSo.MaLoaiHs = maLoaiHs;
            hoSo.DonViHocLx = rq.DonViHocLx;
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

    }
}
