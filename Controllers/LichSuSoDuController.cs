using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ttlaixe.Businesses;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichSuSoDuController : ControllerBase
    {
        private readonly ILichSuSoDuBusiness _business;

        public LichSuSoDuController(ILichSuSoDuBusiness business)
        {
            _business = business;
        }

        // GET theo năm
        [HttpGet("{nam}")]
        public async Task<List<LichSuSoDu>> GetByNam(int nam)
        {
            return await _business.GetByNamAsync(nam);
        }

        // POST Upsert
        [HttpPost]
        //[Authorize]
        public async Task<bool> Upsert([FromBody] LichSuSoDu rq)
        {
            return await _business.UpsertAsync(rq);
        }

        // PUT Update
        [HttpPut]
        [Authorize]
        public async Task<bool> Update([FromBody] LichSuSoDu rq)
        {
            return await _business.UpdateAsync(rq);
        }
    }
}
