using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhatKyChungTuController : ControllerBase
    {
        private readonly INhatKyChungTuBusiness _business;

        public NhatKyChungTuController(INhatKyChungTuBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<List<NhatKyChungTuResponse>> GetAll(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            return await _business.GetAllAsync(fromDate, toDate);
        }

        [HttpGet("{idChungTu}")]
        public async Task<NhatKyChungTu?> GetById(int idChungTu)
        {
            return await _business.GetByIdAsync(idChungTu);
        }

        [Authorize]
        [HttpPost]
        public async Task Create([FromBody] NhatKyChungTuRequest model)
        {
            await _business.CreateAsync(model);
        }

        [Authorize]
        [HttpPut("{idChungTu}")]
        public async Task<bool> Update(int idChungTu, [FromBody] NhatKyChungTu model)
        {
            return await _business.UpdateAsync(idChungTu, model);
        }

        [Authorize]
        [HttpDelete("{idChungTu}")]
        public async Task<bool> Delete(int idChungTu)
        {
            return await _business.DeleteAsync(idChungTu);
        }

        [HttpGet("tong-hop-theo-tai-khoan-chi-tiet")]
        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTiet()
        {
            return await _business.TongHopTheoTaiKhoanChiTietAsync();
        }

        [HttpGet("tong-hop-theo-tai-khoan-cha")]
        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanCha()
        {
            return await _business.TongHopTheoTaiKhoanChaAsync();
        }

        [HttpGet("tong-hop-theo-tai-khoan-chi-tiet-thoi-gian")]
        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTiet(
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate)
        {
            return await _business.TongHopTheoTaiKhoanChiTietAsync(fromDate, toDate);
        }

        [HttpGet("tong-hop-theo-tai-khoan-cha-theo-thoi-gian")]
        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanCha(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            return await _business.TongHopTheoTaiKhoanChaAsync(fromDate, toDate);
        }

        [HttpPost("file-hoa-don-nop-tien-hoc-phi")]
        public async Task<IActionResult> GetChungTuNopHocPhiHV([FromQuery] DateTime fromDate,
    [FromQuery] DateTime toDate)
        {
            var bytes = await _business.GetChungTuNopHocPhiHV(fromDate, toDate);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"HoaDon_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
    }
}
