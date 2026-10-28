using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ttlaixe.DriverApi;

namespace Ttlaixe.Controllers
{
    /// <summary>
    /// Read-only driver queries for partner systems. Same contract as the NestJS sonhung service
    /// (camelCase JSON, validation errors as 400), protected by an IP allowlist instead of a login.
    /// See docs/driver-query-api.md.
    /// </summary>
    [ApiController]
    [Route("api/v1/drivers")]
    [AllowAnonymous]
    [TypeFilter(typeof(DriverApiIpFilter))]
    public class DriversController : ControllerBase
    {
        private readonly IDriversBusiness _business;

        public DriversController(IDriversBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DriverPageResponse), 200)]
        public async Task<IActionResult> Find([FromQuery] DriversQueryRequest _)
        {
            var errors = new List<string>();
            var criteria = DriverRequestValidator.ValidateSearch(Request.Query, errors);
            if (errors.Count > 0)
            {
                return BadRequest(new DriverApiError(errors));
            }

            return Ok(await _business.FindAsync(criteria));
        }

        /// <summary>POST so the identity number stays out of URLs and access logs.</summary>
        [HttpPost("verify")]
        [ProducesResponseType(typeof(DriverVerifyResponse), 200)]
        public async Task<IActionResult> Verify([FromBody] DriverVerifyRequest request)
        {
            var errors = new List<string>();
            DriverRequestValidator.ValidateVerify(request, errors);
            if (errors.Count > 0)
            {
                return BadRequest(new DriverApiError(errors));
            }

            return Ok(await _business.VerifyAsync(request));
        }

        /// <summary>POST so the MaDK list and the identity numbers stay out of URLs and access logs.</summary>
        [HttpPost("profiles")]
        [ProducesResponseType(typeof(DriverProfilesResponse), 200)]
        public async Task<IActionResult> Profiles([FromBody] DriverProfilesRequest request)
        {
            var errors = new List<string>();
            DriverRequestValidator.ValidateProfiles(request, errors);
            if (errors.Count > 0)
            {
                return BadRequest(new DriverApiError(errors));
            }

            Response.Headers["Cache-Control"] = "no-store";
            return Ok(await _business.GetProfilesAsync(request.MaDKs));
        }
    }
}
