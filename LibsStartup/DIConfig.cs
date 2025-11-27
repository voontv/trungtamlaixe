using Microsoft.Extensions.DependencyInjection;
using Ttlaixe.AutoConfig;
using Ttlaixe.FileManager;
using Ttlaixe.Handlers;

namespace Ttlaixe.LibsStartup
{
    public static class DIConfig
    {
        public static void RegisterDI(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            AutoConfigScanner.Scan(services, typeof(AutoConfigScanner));
            services.AddSingleton<TokenProviderMiddleware>();
            // services.AddTransient<IFileService, FileService>();
            services.AddTransient<IFileService, FileService>();
        }
    }
}