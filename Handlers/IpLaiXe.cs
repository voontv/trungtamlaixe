using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ttlaixe.Handlers
{
    public class IpLaiXe : ActionFilterAttribute
    {
        private readonly log4net.ILog _logger
           = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _safelist;

        public IpLaiXe(string safelist)
        {
            _safelist = safelist;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var realP = context.HttpContext.Request.Headers["X-Real-IP"];
            var ip = _safelist.Split(';');
            var badIp = true;
            foreach (var address in ip)
            {
                if (address.Equals(realP))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                _logger.Info("ClientIpCheckActionFilter ClientIpCheckActionFilter IP không cho phép realP ==  " + realP);
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
            _logger.Info("ClientIpCheckActionFilter ClientIpCheckActionFilter IP bạn ok được phép truy cập");
            base.OnActionExecuting(context);
        }
    }
}
