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

        //// POST: api/KhoaHocs
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<KhoaHocResponse> PostKhoaHoc(KhoaHocCreateRequest khoaHoc)
        //{
        //    return await _bs.PostKhoaHoc(khoaHoc);
        //}

        // POST: api/KhoaHocs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("danh-sach-khoa-hoc-theo-thoi-gian")]
        public async Task<List<KhoaHocResponse>> GetListKhoaHocsTheoTg(MocThoiGian dk)
        {
            return await _bs.GetListKhoaHocsTheoTg(dk);
        }

        [HttpPost("danh-sach-khoa-hoc-theo-hang-dao-tao")]
        public async Task<List<KhoaHocResponse>> GetListKhoaHocsTheoHangMucDT(HangDaoTao dk)
        {
            return await _bs.GetListKhoaHocsTheoHangMucDT(dk);
        }

        //// POST: api/KhoaHocs
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost("danh-sach-khoa-hoc-chua-co-lich-hoc")]
        //public async Task KhoaHocChuaTaoLichHoc()
        //{
        //     await _bs.PostKhoaHocTam();
        //}

       
    }
}
