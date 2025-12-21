
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(DanhMucsBusinesses))]
    public interface IDanhMucsBusinesses
    {
        Task<List<DmDiemSatHach>> GetDmDiemSatHach(string hang);
        Task<List<DmDvhcResponse>> GetDmDonViHanhChinh();
        Task<List<DmHangDaoTaoResponse>> GetDmThongTinHangDaoTao();
        Task<List<HangGplxDto>> GetHangGPLX();
        Task<List<HangDaoTaoReponse>> GetHangDaoTao(string maHangGplx);
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

        public async Task<List<HangGplxDto>> GetHangGPLX()
        {
            return await _context.DmHangGplxes
                .Where(x => x.TrangThai == true)
                .Select(x => new HangGplxDto
                {
                    hangGplx = x.MaHang,
                    MaHangGplxMoi = x.MaHangMoi
                })
                .ToListAsync();
        }

        public async Task<List<HangDaoTaoReponse>> GetHangDaoTao(string maHangGplx)
        {
            return await _context.DmHangDts.Where(x => x.HangGplx == maHangGplx)
               .Select(x => new HangDaoTaoReponse
               {
                    MaHangDt = x.MaHangDt,
                   TenHangDt =x.TenHangDt
               }).ToListAsync();
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
