using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.response;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmTaiKhoanKeToanController : ControllerBase
    {
        private readonly IDmTaiKhoanKeToanBusiness _business;

        public DmTaiKhoanKeToanController(IDmTaiKhoanKeToanBusiness business)
        {
            _business = business;
        }

        [HttpGet("tree")]
        public async Task<List<DmTaiKhoanKeToanTreeDto>> GetTree()
        {
            return await _business.GetTreeAsync();
        }

        [HttpGet("tree-by-loai")]
        public async Task<List<DmTaiKhoanKeToanTreeDto>> GetTreeByLoai([FromQuery] string loaiTaiKhoan)
        {
            return await _business.GetTreeByLoaiAsync(loaiTaiKhoan);
        }
    }
}
