using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiLxHoSoesController : ControllerBase
    {
        private readonly GplxCsdtContext _context;

        public NguoiLxHoSoesController(GplxCsdtContext context)
        {
            _context = context;
        }
    }
}
