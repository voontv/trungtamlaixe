using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.request;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XeTapsController : ControllerBase
    {
        private readonly IXeTapsBusinesses _bus;

        public XeTapsController(IXeTapsBusinesses bus)
        {
            _bus = bus;
        }

        [HttpGet("get-all")]
        public Task<List<XeTap>> GetAll()
        {
            return _bus.GetAllAsync();
        }

        [HttpGet("get-by-id")]
        public Task<XeTap> GetById([FromQuery] string bienSoXe)
        {
            return _bus.GetByIdAsync(bienSoXe);
        }

        [Authorize]
        [HttpPost("create")]
        public Task<XeTap> Create([FromBody] XeTapCreatedRequest rq)
        {
            return _bus.CreateAsync(rq);
        }

        [Authorize]
        [HttpPut("update")]
        public Task<bool> Update([FromBody] XeTap rq)
        {
            return _bus.UpdateAsync(rq);
        }

        [Authorize]
        [HttpDelete("delete")]
        public Task<bool> Delete([FromQuery] string bienSoXe)
        {
            return _bus.DeleteAsync(bienSoXe);
        }

        [HttpPost("search")]
        public Task<List<XeTap>> Search([FromBody] XeTapSearchRequest rq)
        {
            return _bus.SearchAsync(rq.Keyword, rq.MaCsdt, rq.MaSoGtvt, rq.TrangThai);
        }
    }
}
