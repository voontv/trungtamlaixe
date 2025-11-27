using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Ttlaixe.Handlers
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                var message = context.ModelState.Values.First(x => x.ValidationState == ModelValidationState.Invalid).Errors[0].ErrorMessage;
                context.Result = new BadRequestObjectResult(message);
            }
        }
    }
}