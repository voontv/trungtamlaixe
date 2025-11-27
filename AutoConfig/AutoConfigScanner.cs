using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Ttlaixe.AutoConfig
{
    public class AutoConfigScanner
    {
        public static void Scan(IServiceCollection services, Type targetType)
        {
            foreach (var type in targetType.Assembly.GetTypes())
            {
                if (type.GetCustomAttribute<ImplementByAttribute>() is { } implementByAttribute)
                {
                    services.AddScoped(type, implementByAttribute.Type);
                }
            }
            // services.AddScoped(typeof(ILoggerManager<>), typeof(LoggerManager<>));
        }
    }
}