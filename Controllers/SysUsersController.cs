using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ttlaixe.Models;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.response;
using Ttlaixe.DTO.request;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysUsersController : ControllerBase
    {
        private readonly ISysUsersBusinesses _business;

        public SysUsersController(ISysUsersBusinesses business)
        {
            _business = business;
        }


    }
}
