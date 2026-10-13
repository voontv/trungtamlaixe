using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ttlaixe.Businesses;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiLxHoSoesController : ControllerBase
    {
        private readonly GplxCsdtContext _context;

        private readonly INguoiLxHoSoesBusinesses _business;

        public NguoiLxHoSoesController(INguoiLxHoSoesBusinesses business)
        {
            _business = business;
        }
        /// <summary>
        /// Import bảng điểm Excel theo MÃ HỌC VIÊN (MaDK).
        /// File gửi multipart/form-data, field name = file.
        /// </summary>
        [HttpPost("ImportBangDiem")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportBangDiem([FromForm] IFormFile file)
        {
            var result = await _business.ImportBangDiemAsync(file);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
