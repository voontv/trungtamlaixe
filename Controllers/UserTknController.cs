using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ttlaixe.Businesses;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;

namespace Ttlaixe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserTknController : ControllerBase
    {
        private readonly IUserTknBusinesses _business;

        public UserTknController(IUserTknBusinesses business)
        {
            _business = business;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public Task<UserTknLoginResponse> Login([FromBody] UserTknLoginRequest request)
            => _business.LoginAsync(request);

        //[Authorize]
        [HttpPost]
        public Task<UserTknResponse> Post([FromBody] UserTknCreateRequest request)
            => _business.CreateAsync(request);

        [Authorize]
        [HttpPut("{userName}")]
        public Task<UserTknResponse> Put(string userName, [FromBody] UserTknUpdateRequest request)
            => _business.UpdateAsync(userName, request);

        [Authorize]
        [HttpDelete("{userName}")]
        public Task<bool> Delete(string userName)
            => _business.DeleteAsync(userName);
    }
}
