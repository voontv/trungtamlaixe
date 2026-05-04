namespace Ttlaixe.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ttlaixe.Businesses;
    using Ttlaixe.DTO.request;
    using Ttlaixe.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class HoSoHocPhiController : ControllerBase
    {
        private readonly IHoSoHocPhiBusiness _business;

        public HoSoHocPhiController(IHoSoHocPhiBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<List<HoSoHocPhi>> GetAll()
        {
            return await _business.GetAllAsync();
        }

        [HttpPost("danh-sach-hoc-phi-khoa-hoc")]
        public async Task<List<HoSoHocPhi>> CreateByKhoaHocAsync([FromBody] DsHocPhiKhoaHocRequest dk)
        {
            return await _business.CreateByKhoaHocAsync(dk.MaKhoaHoc, dk.HangDt);
        }

        [HttpPost("by-ma-khoa-hocs")]
        public async Task<List<HoSoHocPhi>> GetAllByMaKhoaHocs([FromBody] List<string> maKhoaHocs)
        {
            return await _business.GetAllByMaKhoaHocsAsync(maKhoaHocs);
        }

        [HttpPost("chua-hoan-thanh/by-ma-khoa-hocs")]
        public async Task<List<HoSoHocPhi>> GetChuaHoanThanhByMaKhoaHocs([FromBody] List<string> maKhoaHocs)
        {
            return await _business.GetChuaHoanThanhByMaKhoaHocsAsync(maKhoaHocs);
        }

        [HttpPost("da-hoan-thanh/by-ma-khoa-hocs")]
        public async Task<List<HoSoHocPhi>> GetDaHoanThanhByMaKhoaHocs([FromBody] List<string> maKhoaHocs)
        {
            return await _business.GetDaHoanThanhByMaKhoaHocsAsync(maKhoaHocs);
        }

        [HttpGet("chua-hoan-thanh")]
        public async Task<List<HoSoHocPhi>> GetChuaHoanThanh()
        {
            return await _business.GetChuaHoanThanhAsync();
        }

        [HttpGet("da-hoan-thanh")]
        public async Task<List<HoSoHocPhi>> GetDaHoanThanh()
        {
            return await _business.GetDaHoanThanhAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<HoSoHocPhi> Create([FromBody] HoSoHocPhi model)
        {
            return await _business.CreateAsync(model);
        }

        [Authorize]
        [HttpPut("{maDK}")]
        public async Task<bool> Update(string maDK, [FromBody] HoSoHocPhi model)
        {
            return await _business.UpdateAsync(maDK, model);
        }

        [Authorize]
        [HttpPut("cap-nhat-trang-thai-thanh-toan/{maDK}")]
        public async Task<bool> UpdateTrangThaiThanhToan(string maDK)
        {
            return await _business.UpdateTrangThaiThanhToanAsync(maDK);
        }

        [Authorize]
        [HttpPut("bo-hoc/{maDK}")]
        public async Task<bool> BoHoc(string maDK)
        {
            return await _business.BoHocAsync(maDK);
        }
    }
}
