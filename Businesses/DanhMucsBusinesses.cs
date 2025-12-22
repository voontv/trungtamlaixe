
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.Exceptions;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(DanhMucsBusinesses))]
    public interface IDanhMucsBusinesses
    {
        Task<List<DmDiemSatHach>> GetDmDiemSatHach(string hang);
        Task<List<DmDvhcResponse>> GetDmDonViHanhChinh();
        Task<List<DmHangDaoTaoResponse>> GetDmThongTinHangDaoTao();
        Task<List<string>> GetMaHangDaoTao();
        Task<List<HangDaoTaoReponse>> GetLoaiHinhDaoTao(string maHangGplx);
        Task<List<DmLoaiHsoResponse>> GetDmLoaiHso();
        Task<List<DmLoaiHsoGiayToResponse>> GetDmLoaiHsoGiayTo(string maHangGPLX);

        Task<List<DmQuocTich>> GetDMQuocTich();

        Task<List<DmTenKeHoachDaoTaoItem>> GetDanhMucKhdt();

    }
    public class DanhMucsBusinesses : ControllerBase, IDanhMucsBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly IAuthenInfo _authenInfo;
        public DanhMucsBusinesses(GplxCsdtContext context, IAuthenInfo authenInfo)
        {
            _context = context;
            _authenInfo = authenInfo;
        }
        
        public async Task<List<DmDiemSatHach>> GetDmDiemSatHach(string hang)
        {
            return await _context.DmDiemSatHaches.Where(x => x.Hang == hang).ToListAsync();
        }

        public async Task<List<DmDvhcResponse>> GetDmDonViHanhChinh()
        {

            return await _context.DmDvhcs.Where(x => x.TrangThai == true).Select(x => new DmDvhcResponse
            {
                MaDvhc = x.MaDvhc,
                MaDvql = x.MaDvql,
                MaDv = x.MaDv,
                TenDvhc = x.TenDvhc,
                TenNganGon = x.TenNganGon,
                TenDayDu = x.TenDayDu
            })
                .ToListAsync();
        }

        public async Task<List<DmHangDaoTaoResponse>> GetDmThongTinHangDaoTao()
        {
            return await _context.DmHangDts.Where(x => x.TrangThai == true).Select(x => new DmHangDaoTaoResponse 
            {
                MaHangDt = x.MaHangDt,
                TenHangDt = x.TenHangDt,
                HangGplx = x.HangGplx,
                SoVbpl = x.SoVbpl,
                TuoiHv = x.TuoiHv,
                ThamNien = x.ThamNien
            })
                .ToListAsync();
        }

        public async Task<List<DmLoaiHsoGiayToResponse>> GetDmLoaiHsoGiayTo(string maHangGPLX)
        {
            return await _context.DmLoaiHsoGiayTos.Where(x => x.TrangThai == true && x.MaHangGplx == maHangGPLX)
                .Select(x => new DmLoaiHsoGiayToResponse
                {
                    MaGt = x.MaGt,
                    MaLoaiHs = x.MaLoaiHs,
                    MaHangGplx = x.MaHangGplx,
                    TenGt = x.TenGt
                })
                .ToListAsync();
        }

        public async Task<List<string>> GetMaHangDaoTao()
        {
            return await _context.DmHangDts
                .Where(x => x.TrangThai == true)
                .Select(x => x.HangGplx).Distinct()
                .ToListAsync();
        }

        public async Task<List<HangDaoTaoReponse>> GetLoaiHinhDaoTao(string maHangDaoTao)
        {
            maHangDaoTao = (maHangDaoTao ?? "").Trim();
            if (string.IsNullOrEmpty(maHangDaoTao))
                return new List<HangDaoTaoReponse>();

            // Lấy tất cả hạng đào tạo theo HangGPLX (mã mới) đang active
            var dts = await _context.DmHangDts
                .Where(x => x.TrangThai == true && x.HangGplx == maHangDaoTao)
                .Select(x => new { x.MaHangDt, x.TenHangDt, x.HangGplx })
                .ToListAsync();

            if (dts.Count == 0)
                return new List<HangDaoTaoReponse>();

            // Map HangGPLX (mã mới) -> 1 mã đích (MaHang) theo thứ tự ổn định (MaHang)
            // (B -> lấy B1 vì B1 < B2 theo ORDER BY MaHang)
            var hangMoiSet = dts.Select(x => x.HangGplx).Distinct().ToList();

            var map = await _context.DmHangGplxes
                .Where(g => hangMoiSet.Contains(g.MaHangMoi) || hangMoiSet.Contains(g.MaHang))
                .Select(g => new { g.MaHangMoi, g.MaHang })
                .ToListAsync();

            // chọn 1 đích cho mỗi mã mới: Min/MaHang đầu tiên theo order
            var dict = map
                .GroupBy(x => x.MaHangMoi)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.MaHang).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).FirstOrDefault()
                );

            var result = new List<HangDaoTaoReponse>();

            foreach (var dt in dts)
            {
                // Ưu tiên map theo MaHangMoi, nếu không có thì thử dt.HangGplx vốn đã là MaHang
                dict.TryGetValue(dt.HangGplx, out var maDich);

                if (string.IsNullOrWhiteSpace(maDich))
                    maDich = dt.HangGplx; // fallback (tuỳ bạn có muốn hay không)

                result.Add(new HangDaoTaoReponse
                {
                    MaGplx = maDich,
                    TenHangDt = dt.TenHangDt
                });
            }

            return result;
        }


        public async Task<List<DmLoaiHsoResponse>> GetDmLoaiHso()
        {
            var hsos = new List<DmLoaiHsoResponse>();
            var result = await _context.DmLoaiHsos.Where(x => x.TrangThai == true).ToListAsync();
            result.Patch(hsos);
            return hsos;
        }

        public async Task<DmHangDt> GetMaHangDaoTao(string MaHangDT)
        {
            return await _context.DmHangDts.FindAsync(MaHangDT);
        }

        public async Task<List<DmQuocTich>> GetDMQuocTich()
        {
            return await _context.DmQuocTiches.Where(x => x.TrangThai == true).ToListAsync();
        }

        public Task<List<DmTenKeHoachDaoTaoItem>> GetDanhMucKhdt()
        {
            /*var list = new List<DmTenKeHoachDaoTaoItem>
            {  new DmTenKeHoachDaoTaoItem { MaDaoTao = "LT", TenDaoTao = "Lý thuyết" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "TH", TenDaoTao = "Thực hành hình" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "TD", TenDaoTao = "Thực hành đường" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "KT", TenDaoTao = "Thi kiểm tra" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "NG", TenDaoTao = "Nghỉ" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "DU", TenDaoTao = "Dự phòng" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "SH", TenDaoTao = "Sát hạch" },};*/

            var list = new List<DmTenKeHoachDaoTaoItem>
            {  new DmTenKeHoachDaoTaoItem { MaDaoTao = "L", TenDaoTao = "Lý thuyết" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "H", TenDaoTao = "Thực hành hình" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "D", TenDaoTao = "Thực hành đường" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "KT", TenDaoTao = "Thi kiểm tra" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "NG", TenDaoTao = "Nghỉ" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "DU", TenDaoTao = "Dự phòng" },
                new DmTenKeHoachDaoTaoItem { MaDaoTao = "SH", TenDaoTao = "Sát hạch" }};

            return Task.FromResult(list);
        }

    }
}
