using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Ttlaixe.Handlers
{
    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _safelist;
        private static readonly log4net.ILog log
           = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AdminSafeListMiddleware(
            RequestDelegate next,
            string safelist)
        {
            var ips = safelist.Split(';');
            log.Info("AdminSafeListMiddleware  AdminSafeListMiddleware Day IP trong config la ips     " + safelist);
            _safelist = ips;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            /*
            if (context.ThongTinNguoiHocLai.Method != HttpMethod.Get.Method)
            {
                var realP = context.ThongTinNguoiHocLai.Headers["X-Real-IP"];
                log.Info("AdminSafeListMiddleware IP may nhan ra tai Invoke la    X_Real_IP " + realP);
                var badIp = true;
                foreach (var address in _safelist)
                {
                    log.Info("AdminSafeListMiddleware IP may nhan ra tai Invoke la    address " + address + "  bytes "
                            + realP);
                    if (address.Equals(realP))
                    {
                        log.Info("Khi duoc cho phep day IP  address " + address + "  bytes "
                            + realP);
                        badIp = false;
                        break;
                    }
                }

                if (badIp)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }
            log.Info("Day IP ok  _safelist   " + _safelist);
            */
            await _next.Invoke(context);
        }
    }
}
