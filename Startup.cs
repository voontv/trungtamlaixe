using BusinessLogic.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ttlaixe.Handlers;
using Ttlaixe.LibsStartup;
using Ttlaixe.FileManager;
using Ttlaixe.Providers;
using System;
using System.Text;
using System.Text.Json.Serialization;
using Ttlaixe.Models;
using Ttlaixe.DTO.request;

namespace Ttlaixe
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.OutputEncoding = Encoding.UTF8;
            services.AddControllers();
            //services.ConfigSecurity(Providers<SecuritySettings>(services));
            services.ConfigSecurity(Config<SecuritySettings>(services));
            //services.AddAuthorization();
            services.Configure<UploadOptions>(Configuration.GetSection("Upload"));

            services.Configure<WebConfig>(Configuration.GetSection("WebConfig"));
            services.AddDbContext<GplxCsdtContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Ttlaixe")));
            //services.AddScoped<ISFTPFileService, SFTPFileService>();
            services.RegisterDI();
            services.AddMvc(FilterHelper.Register).AddJsonOptions(ConfigJson);
#if DEBUG
            //swagger
            services.AddSwaggerGen(SwaggerConfig.ConfigSwagger);
#endif

            services.AddScoped<IpLaiXe>(container =>
            {

                return new IpLaiXe(Configuration.GetSection("AdminSafeList").GetSection("Vnpt").Value);
            });
        }


        private static void ConfigJson(JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeJsonNamingPolicy();
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        }

        private T Config<T>(IServiceCollection services) where T : class
        {
            var config = Activator.CreateInstance<T>();
            Configuration.Bind(typeof(T).Name, config);
            services.AddSingleton(config);
            return config;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, /*QlcongVanContext dataContext,*/ ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddLog4Net();
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(CheckAllowOrigin) // allow any origin
                .AllowCredentials()); // allow credentials
            app.UseMiddleware<TokenProviderMiddleware>();
            //keep
            //app.UseMiddleware<TokenProviderMiddleware>();
            //app.UseHttpsRedirection();

            app.UseRouting();
            //app.UseAuthentication();
            app.UseAuthorization();
#if DEBUG
            //swagger
            app.ConfigSwagger();
#endif


            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            app.UseRewriter(new RewriteOptions().AddRewrite(@"^((?!.*?\b(web$.*|api\/.*)))((\w+))*\/?(\.\w{{5,}})?\??([^.]+)?$", "index.html", false));
            app.UseDefaultFiles(options);

            app.UseStaticFiles();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            //app.UseHttpsRedirection();
        }

        private bool CheckAllowOrigin(string origin)
        {
            return true;
        }
    }
}
