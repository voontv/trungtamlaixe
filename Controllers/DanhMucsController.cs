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

        [HttpGet("hang-dao-dao")]
        public async Task<List<DmHangDaoTaoResponse>> GetDmHangDaoTao()
        {
            return await _business.GetDmHangDaoTao();
        }

        [HttpGet("loai-ho-so-giay-to/{maHangGPLX}")]
        public async Task<List<DmLoaiHsoGiayToResponse>> GetDmLoaiHsoGiayTo(string maHangGPLX)
        {
            return await _business.GetDmLoaiHsoGiayTo(maHangGPLX);
        }

    
    }
}
