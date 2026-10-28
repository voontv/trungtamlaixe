using BusinessLogic.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
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
using Ttlaixe.Businesses;
using Ttlaixe.DriverApi;

namespace Ttlaixe
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Console.OutputEncoding = Encoding.UTF8;

            services.AddControllers();
            services.ConfigSecurity(Config<SecuritySettings>(services));

            services.Configure<UploadOptions>(Configuration.GetSection("Upload"));
            services.Configure<WebConfig>(Configuration.GetSection("WebConfig"));

            services.AddHttpContextAccessor();
            services.AddScoped<IGplxTenantProvider, GplxTenantProvider>();

            // ✅ QUAN TRỌNG NHẤT — Factory
            services.AddDbContextFactory<GplxCsdtContext>();

            // ✅ Context tạo theo từng request (đọc header đúng)
            services.AddScoped<GplxCsdtContext>(sp =>
            {
                var tenant = sp.GetRequiredService<IGplxTenantProvider>();
                var conn = tenant.GetConnectionString();

                var options = new DbContextOptionsBuilder<GplxCsdtContext>()
                    .UseSqlServer(conn)
                    .Options;

                return new GplxCsdtContext(options);
            });

            services.AddDbContext<TeknovaContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Teknova")));

            services.RegisterDI();

            services.AddMvc(FilterHelper.Register)
                .AddJsonOptions(ConfigJson);

#if DEBUG
            services.AddSwaggerGen(SwaggerConfig.ConfigSwagger);
#endif

            services.AddScoped<IpLaiXe>(container =>
            {
                return new IpLaiXe(
                    Configuration.GetSection("AdminSafeList")
                    .GetSection("Vnpt").Value);
            });

            services.AddScoped<IImageGplxService, ImageGplxService>();
            services.AddScoped<INguoiLxesBusinesses, NguoiLxesBusinesses>();
            services.AddScoped<IHoSoHocPhiBusiness, HoSoHocPhiBusiness>();
            services.AddScoped<IHoSoHocPhiBusiness, HoSoHocPhiBusiness>();
            services.AddScoped<IKhoaHocsBusinesses, KhoaHocsBusinesses>();
            services.AddScoped<IDriversBusiness, DriversBusiness>();
            services.AddSingleton<IDriverPhotoPathResolver, DirectPhotoPathResolver>();
            services.AddSingleton<IDriverPhotoService, DriverPhotoService>();
        }

        private static void ConfigJson(JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeJsonNamingPolicy();
            options.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        }

        private T Config<T>(IServiceCollection services) where T : class
        {
            var config = Activator.CreateInstance<T>();
            Configuration.Bind(typeof(T).Name, config);
            services.AddSingleton(config);
            return config;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddLog4Net();

            // Behind a TLS-terminating proxy (Cloudflare Tunnel, IIS ARR...) the app only sees plain HTTP.
            // Read X-Forwarded-Proto/For first, otherwise the redirect below loops forever and
            // DriverApi:AllowedIps is matched against the proxy instead of the real client.
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<TokenProviderMiddleware>();

#if DEBUG
            app.ConfigSwagger();
#endif

            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            app.UseRewriter(new RewriteOptions().AddRewrite(
                @"^((?!.*?\b(web$.*|api\/.*)))((\w+))*\/?(\.\w{{5,}})?\??([^.]+)?$",
                "index.html",
                false));

            app.UseDefaultFiles(options);
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}