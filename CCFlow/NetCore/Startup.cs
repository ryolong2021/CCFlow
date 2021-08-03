using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using BP.Web;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace CCFlow
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private string crosPolicyName = "allow-specific-originals";

        /// <summary>
        /// 配置服务将服务注册到依赖注入容器中。
        /// 在Configure()之前执行。
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // 看这篇文章，以理解下面的跨域配置 https://docs.microsoft.com/zh-cn/aspnet/core/security/cors?view=aspnetcore-3.0
            services.AddCors(options =>
            {
                options.AddPolicy(crosPolicyName,
                builder =>
                {
                    builder.WithOrigins("http://localhost:8000",
                                        "https://localhost:8000") // 指定前端地址
                    .AllowAnyHeader() // 用于支持http头中带token的认证
                    .AllowAnyMethod() // 跨域时，浏览器会发一个OPTIONS请求。这儿也可以用.WithHeaders来指定具体的Http方法
                    .SetPreflightMaxAge(TimeSpan.FromDays(1)) // 将预检请求的结果缓存, 即设置Access-Control-Max-Age头
                    .AllowCredentials(); // 设置 Access-Control-Allow-Credentials 标头
                });
            });

            services.AddControllers()
                .AddNewtonsoftJson()
                .ConfigureApplicationPartManager(m => {
                    var feature = new ControllerFeature();
                    m.PopulateFeature(feature);
                    services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
                });

            // Session默认用的是DistributedMemory
            services.AddDistributedMemoryCache();

            // 如果不使用cshtml，则不需要此服务
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // 用于类库中访问HttpContext，等价于： services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();

            // 配置cookie (不跨域可以不配)
            services.Configure<CookiePolicyOptions>(options =>
            {
                // 因为asp.net core 2.1 开始遵循欧盟GDPR规则，所以需要启用下面的两个配置
                // 参考文章： https://andrewlock.net/session-state-gdpr-and-non-essential-cookies/
                // 是否需要用户手动确认接受cookie，配置为false，表示不需要用户确认
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                // options.SameSite = SameSiteMode.None; 
            });

            // CookieAuthenticationOptions.TicketDataFormat 使用
            // 如果使用了 services.AddAuthentication() 或 services.AddSession()，那么此处就无需单独调用services.AddDataProtection()
            //services.AddDataProtection();

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true; // 不跨域可以不配
                options.Cookie.SameSite = SameSiteMode.Strict;

                options.IdleTimeout = TimeSpan.FromHours(24);
                options.IOTimeout = TimeSpan.FromSeconds(5);
            });

            // 解决异常：System.InvalidOperationException: Synchronous operations are disallowed. Call WriteAsync or set AllowSynchronousIO to true instead.
            // 原因参见： https://github.com/aspnet/AspNetCore/issues/8302
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        /// <summary>
        /// 定义应用程序如何响应HTTP请求，以及配置HTTP流水线中的中间件
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 开发环境启用异常显示页面
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // 跨域
            app.UseCors(crosPolicyName);

            // 启用全局Cookie配置
            app.UseCookiePolicy();

            // 使用静态文件访问
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),

            });

            //app.UseStaticFiles();
            // 使用路由
            app.UseRouting();

            // 使用Session
            app.UseSession();

            // ccflow核心中间件
            app.UseCcHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area}/{controller}/{action=Index}/{id?}");

            });

            // 用于在非Controller之类的Http请求中，以静态方式获取到HttpContext。通过HttpContextHelper静态辅助类。
            app.UseStaticHttpContext();

            // En30里面用到
            HttpContextHelper.PhysicalApplicationPath = env.ContentRootPath + Path.DirectorySeparatorChar;

            NetCoreAppHelper.ServiceProvider = app.ApplicationServices;
        }
    }
}
