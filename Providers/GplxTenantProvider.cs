using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace Ttlaixe.Providers
{
    public interface IGplxTenantProvider
    {
        string GetConnectionString();
    }
    public class GplxTenantProvider : IGplxTenantProvider
    {
        private readonly IHttpContextAccessor _http;
        private readonly IConfiguration _config;

        public GplxTenantProvider(IHttpContextAccessor http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public string GetConnectionString()
        {
            var header = _http.HttpContext?.Request.Headers["x-loai-xe"].ToString();

            if (header?.Equals("moto", StringComparison.OrdinalIgnoreCase) == true)
                return _config.GetConnectionString("Gplx_XeMay");

            return _config.GetConnectionString("Gplx_Oto");
        }
    }
}
