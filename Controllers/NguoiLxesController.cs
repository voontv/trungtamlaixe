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
    public class NguoiLxesController : ControllerBase
    {
        private readonly INguoiLxesBusinesses _business;

        public NguoiLxesController(INguoiLxesBusinesses business)
        {
            _business = business;
        }

        [HttpPost]
        public async Task<NguoiLxResponse> CreateAsync(NguoiLxCreateRequest request)
        {
            return await _business.CreateAsync(request);
        }

        [HttpPut]
        public async Task<bool> UpdateAsync(NguoiLxResponse request)
        {
            return await _business.UpdateAsync(request);
        }

    }
}
