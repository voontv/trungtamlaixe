using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ttlaixe.Businesses;
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

        

        
    }
}
