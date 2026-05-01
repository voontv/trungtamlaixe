namespace Ttlaixe.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ttlaixe.Businesses;
    using Ttlaixe.DTO.request;
    using Ttlaixe.DTO.response;
    using Ttlaixe.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class LichSuNopHocPhiController : ControllerBase
    {
        private readonly ILichSuNopHocPhiBusiness _business;

        public LichSuNopHocPhiController(ILichSuNopHocPhiBusiness business)
        {
            _business = business;
        }

        [HttpGet("by-ma-dk/{maDK}")]
        public async Task<List<LichSuNopHocPhiReponse>> GetByMaDK(string maDK)
        {
            return await _business.GetByMaDKAsync(maDK);
        }
        
        //[Authorize]
        [HttpPost]
        public async Task<LichSuNopHocPhiReponse> Create([FromBody] LichSuNopHocPhiRequest model)
        {
            return await _business.CreateAsync(model);
        }

        //[Authorize]
        [HttpDelete("{idNopTien}")]
        public async Task<bool> Delete(int idNopTien)
        {
            return await _business.DeleteAsync(idNopTien);
        }
    }
}
