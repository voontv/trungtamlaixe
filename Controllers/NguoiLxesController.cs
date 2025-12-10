using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiLxesController : ControllerBase
    {
        private readonly INguoiLxesBusinesses _business;

        public NguoiLxesController(INguoiLxesBusinesses business)
        {
            _business = business;
        }

        [HttpGet("danh-sach-hoc-vien/{maKH}")]
        public async Task<List<NguoiLxCoBanResponse>> GetThongTinCoBanByKhoaHocAsync(string maKH)
        {
            return await _business.GetThongTinCoBanByKhoaHocAsync(maKH);
        }

        [HttpGet("danh-sach-hoc-vien-sat-hach/{MaSatHach}")]
        public async Task<List<NguoiLxThiResponse>> GetThongTinThiSinhCoTheThiAsync(string MaSatHach)
        {
            return await _business.GetDanhSachSatHach(MaSatHach);
        }

        [HttpGet("thong-tin-nguoi-lai-xe/{maDK}")]
        public async Task<NguoiLxResponse> GetThongTinNguoiLx(string maDK)
        {
            return await _business.GetThongTinNguoiLx(maDK);
        }

        [HttpPost]
        public async Task<NguoiLxResponse> CreateAsync(NguoiLxCreateRequest request)
        {
            return await _business.CreateAsync(request);
        }

        [HttpPut]
        public async Task<bool> UpdateAsync(NguoiLxResponse request)
        {
            return await _business.UpdateAsync(request);
        }

    }
}
