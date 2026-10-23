using System;
using System.Linq;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Ttlaixe.DriverApi
{
    /// <summary>
    /// Only lets the IPs in DriverApi:AllowedIps (separated by ";", exact match) call the driver endpoints.
    /// Fails closed: an empty or missing list rejects everyone. The client IP is the X-Real-IP header set by the
    /// reverse proxy (it must overwrite any value a client sends), else the connection's remote address.
    /// </summary>
    public class DriverApiIpFilter : IAuthorizationFilter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DriverApiIpFilter));

        private readonly IConfiguration _config;

        public DriverApiIpFilter(IConfiguration config)
        {
            _config = config;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowed = (_config["DriverApi:AllowedIps"] ?? "")
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var ip = ClientIp(context.HttpContext);

            if (ip == null || !allowed.Contains(ip, StringComparer.OrdinalIgnoreCase))
            {
                Log.Info($"DriverApiIpFilter rejected IP {ip}");
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }

        private static string ClientIp(HttpContext context)
        {
            var header = context.Request.Headers["X-Real-IP"].ToString().Trim();
            if (header.Length > 0)
            {
                return header;
            }

            var address = context.Connection.RemoteIpAddress;
            if (address == null)
            {
                return null;
            }

            return (address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address).ToString();
        }
    }
}
