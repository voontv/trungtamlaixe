using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class HocVienChuaPhanKhoaController : ControllerBase
    {
        private readonly IHocVienChuaPhanKhoaBusiness _business;

        public HocVienChuaPhanKhoaController(IHocVienChuaPhanKhoaBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<List<HocVienChuaPhanKhoaDTO>> GetAll()
        {
             return await _business.GetAllAsync(true);
        }

        [HttpGet("danh-sach-hoc-vien-da-phan-lop")]
        public async Task<List<HocVienChuaPhanKhoaDTO>> GetAllKhoiPhuc()
        {
            return await _business.GetAllAsync(false);
        }

        [HttpGet("image-by-path")]
        public async Task<IActionResult> GetImageByPath([FromQuery] string path)
        {
            var result = await _business.GetImageByPathAsync(path);
            if (result == null)
                return NotFound();

            Response.Headers["Content-Disposition"] = "inline";

            return File(result.Value.Bytes, result.Value.ContentType);
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task Create(
        [FromForm] HocVienChuaPhanKhoaRequest model)
        {
            await _business.CreateAsync(model);
        }

        [HttpPost("tim-kiem")]
        public async Task<List<HocVienChuaPhanKhoaDTO>> SearchAsync(HocVienChuaPhanKhoaSearchRequest rq)
        {
            return await _business.SearchAsync(rq);
        }

        [HttpPut]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task Update([FromForm] HocVienChuaPhanKhoaRequest model)
        {
            await _business.UpdateAsync(model);
        }

        [HttpGet("thay-doi-tinh-trang-phan-lop/{Idhs}")]
        [Authorize]
        public async Task Update(int Idhs)
        {
            await _business.UpdateTrangThai(Idhs);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _business.DeleteAsync(id);
        }
    }
}
