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
    public class KhoaHocsController : ControllerBase
    {
        private readonly IKhoaHocsBusinesses _bs;

        public KhoaHocsController(IKhoaHocsBusinesses bs)
        {
            _bs = bs;
        }

        // POST: api/KhoaHocs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<KhoaHocResponse> PostKhoaHoc(KhoaHocCreateRequest khoaHoc)
        {
            return await _bs.PostKhoaHoc(khoaHoc);
        }

        // POST: api/KhoaHocs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("danh-sach-khoa-hoc-theo-thoi-gian")]
        public async Task<List<KhoaHocResponse>> GetListKhoaHocsTheoTg(MocThoiGian dk)
        {
            return await _bs.GetListKhoaHocsTheoTg(dk);
        }

        //// DELETE: api/KhoaHocs/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteKhoaHoc(string id)
        //{
        //    var khoaHoc = await _context.KhoaHocs.FindAsync(id);
        //    if (khoaHoc == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.KhoaHocs.Remove(khoaHoc);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

    }
}
