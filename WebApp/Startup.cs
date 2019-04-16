using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Default", // Route name
                    "/png", // URL with parameters
                    new {controller = "Home", action = "Png"} // Parameter defaults
                );
                routes.MapRoute(
                    "OperationUpload", // Route name
                    "/opr/uploadfile", // URL with parameters
                    new {controller = "Operations", action = "UploadFile"} // Parameter defaults
                );
                routes.MapRoute(
                    "SelectRegion",
                    "/analyze/selectregion/{FileName}",
                    new {controller = "Analyze", action = "SelectRegion"},
                    new {FileName = @"\w+"});
                routes.MapRoute(
                    "ShowPng",
                    "/analyze/showPng/{path}",
                    new {controller = "Analyze", action = "GetPngFromTempPath"},
                    new {path = @"\w+"});
                routes.MapRoute(
                    "StartAnalyzing",
                    "/analyze/startAnalyze",
                    new {controller = "Analyze", action = "StartAnalyzing"});
                routes.MapRoute(
                    "StartAnalyzing",
                    "/analyze/showResult/{path}",
                    new {controller = "Analyze", action = "ShowAnalysis"}, new {path = @"\w+"});
                routes.MapRoute(
                    "default",
                    "/",
                    new {controller = "Home", action = "Index"});
            });
        }
    }
}