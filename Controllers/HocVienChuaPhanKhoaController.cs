using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
             return await _business.GetAllAsync();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
        [FromForm] HocVienChuaPhanKhoa model,
        [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Chưa chọn file ảnh.");

            var result = await _business.CreateAsync(model, file);

            return Ok(result);
        }

        [HttpPost("tim-kiem")]
        public async Task<List<HocVienChuaPhanKhoaDTO>> SearchAsync(HocVienChuaPhanKhoaSearchRequest rq)
        {
            return await _business.SearchAsync(rq);
        }

        [HttpPut]
        public async Task Update([FromBody] HocVienChuaPhanKhoa model)
        {
            await _business.UpdateAsync(model);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _business.DeleteAsync(id);
        }
    }
}
