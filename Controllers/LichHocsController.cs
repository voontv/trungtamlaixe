using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.request;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LichHocsController : ControllerBase
    {
        private readonly ILichHocsBusinesses _business;

        public LichHocsController(ILichHocsBusinesses biz)
        {
            _business = biz;
        }

        [HttpGet("get-all")]
        public Task<List<LichHoc>> GetAll()
            => _business.GetAllAsync();

        [HttpGet("{maLichHoc:long}")]
        public Task<LichHoc> GetById(long maLichHoc)
            => _business.GetByIdAsync(maLichHoc);

        [HttpGet("by-ma-kh/{maKh}")]
        public Task<List<LichHoc>> GetByMaKh(string maKh)
            => _business.GetByMaKhAsync(maKh);

        // Search dạng query string:
        // /api/LichHocs/search?maKh=48012K21B2004&thang=5&tuan=9&fromDate=2021-05-01&toDate=2021-05-31
        [HttpGet("search")]
        public Task<List<LichHoc>> Search(
            [FromQuery] string maKh,
            [FromQuery] int? thang,
            [FromQuery] int? tuan,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate
        )
            => _business.SearchAsync(maKh, thang, tuan, fromDate, toDate);

        [HttpPost("create-many")]
        [Authorize]
        public async Task<bool> CreateManyAsync([FromBody] List<LichHocCreatedRequest> rqs)
        {
            return await _business.CreateManyAsync(rqs);
        }

        // PUT: api/LichHocs/update-many
        [HttpPut("update-many")]
        [Authorize]
        public async Task<bool> UpdateManyAsync([FromBody] List<LichHocCreatedRequest> rqs)
        {
            return await _business.UpdateManyAsync(rqs);
        }

        [HttpDelete("delete-by-ma-khoa-hoc/{maKh}")]
        [Authorize]
        public async Task<bool> DeleteByMaKhAsync(string maKh)
        {
            return await _business.DeleteByMaKhAsync(maKh);
        }
    }
}
