using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ttlaixe.Businesses;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiaoViensController : ControllerBase
    {
        private readonly IGiaoViensBusinesses _business;

        public GiaoViensController(IGiaoViensBusinesses business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<List<GiaoVien>> GetAll()
        {
            return await _business.GetAllAsync();
        }

        [HttpGet("{maGv}")]
        public async Task<GiaoVien> GetById(string maGv)
        {
            return await _business.GetByIdAsync(maGv);
        }

        [HttpPost]
        [Authorize]
        public async Task<GiaoVien> Create(GiaoVien request)
        {
            return await _business.CreateAsync(request);
        }

        [HttpPut]
        [Authorize]
        public async Task<bool> Update(GiaoVien request)
        {
            return await _business.UpdateAsync(request);
        }

        [HttpDelete("{maGv}")]
        [Authorize]
        public async Task<bool> Delete(string maGv)
        {
            return await _business.DeleteAsync(maGv);
        }
    }
}
