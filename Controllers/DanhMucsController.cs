using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ttlaixe.Models;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.response;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucsController : ControllerBase
    {
        private readonly IDanhMucsBusinesses _business;

        public DanhMucsController(IDanhMucsBusinesses business)
        {
            _business = business;
        }

        [HttpGet("diem-sat-hach/{hang}")]
        public async Task<List<DmDiemSatHach>> GetDmDiemSatHach(string hang)
        {
            return await _business.GetDmDiemSatHach(hang);
        }
        
        [HttpGet("don-vi-hanh-chinh")]
        public async Task<List<DmDvhcResponse>> GetDmDonViHanhChinh()
        {
            return await _business.GetDmDonViHanhChinh();
        }

        [HttpGet("ma-hang-dao-tao")]
        public async Task<List<string>> GetMaHangDaoTao()
        {
            return await _business.GetMaHangDaoTao();
        }

        [HttpGet("loai-hinh-dao-tao/{maDaoTao}")]
        public async Task<List<HangDaoTaoReponse>> GetDmLoaiHinhDaoTao(string maDaoTao)
        {
            return await _business.GetLoaiHinhDaoTao(maDaoTao);
        }

        [HttpGet("loai-ho-so")]
        public async Task<List<DmLoaiHsoResponse>> GetDmLoaiHso()
        {
            return await _business.GetDmLoaiHso();
        }

        [HttpGet("loai-ho-so-giay-to/{maHangGPLX}")]
        public async Task<List<DmLoaiHsoGiayToResponse>> GetDmLoaiHsoGiayTo(string maHangGPLX)
        {
            return await _business.GetDmLoaiHsoGiayTo(maHangGPLX);
        }

        [HttpGet("loai-quoc-tich")]
        public async Task<List<DmQuocTich>> GetDMQuocTich()
        {
            return await _business.GetDMQuocTich();
        }

        [HttpGet("ten-ke-hoach-dao-tao")]
        public async Task<List<DmTenKeHoachDaoTaoItem>> GetDanhMucKhdt()
        {
            return await _business.GetDanhMucKhdt();
        }

    }
}
