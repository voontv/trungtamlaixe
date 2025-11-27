using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ttlaixe.Models;
using Ttlaixe.Businesses;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucsController : ControllerBase
    {
        private readonly IDanhMucsBusinesses _business;

        public DanhMucsController(IDanhMucsBusinesses business)
        {
            _business = business;
        }

        /* GET: api/BenhAns
        [HttpGet]
        public async Task<List<BenhAn>> GetLstBenhAn()
        {
            return await _business.GetLstBenhAn();
        }

        // GET: api/BenhAns/5
        [HttpGet("{id}")]
        public async Task<object> GetData_BenhAn_Va_Phong_Giuong_ByBenhAn_IdHai(int id)
        {
           
            return await _business.GetData_BenhAn_Va_Phong_Giuong_ByBenhAn_IdHai(id);
        }
        */
    }
}
