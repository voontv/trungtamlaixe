
using Microsoft.AspNetCore.Mvc;
using Ttlaixe.Handlers;

namespace Ttlaixe.LibsStartup
{
    public static class FilterHelper
    {
        public static void Register(this MvcOptions options)
        {
            options.Filters.Add(typeof(UnHandledExceptionHandle));
            options.Filters.Add(typeof(HandledExceptionHandle));
            options.Filters.Add(typeof(ValidateModelAttribute));
        }
    }
}