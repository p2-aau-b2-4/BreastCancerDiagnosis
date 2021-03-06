﻿using Microsoft.AspNetCore.Builder;
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
            //services.Configure<CookiePolicyOptions>(options =>
            //{
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

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

            app.UseMvc(routes =>
            {
                // operationscontroller - upload and showing images
                routes.MapRoute(
                    "OperationUpload", // Route name
                    "/opr/uploadfile", // URL with parameters
                    new {controller = "Operations", action = "UploadFile"} // Parameter defaults
                );
                routes.MapRoute(
                    "ShowPng",
                    "/analyze/showPng/{path}",
                    new {controller = "Operations", action = "ShowSavedPng"},
                    new {path = @"[\w-]+"});
                routes.MapRoute(
                    "ShowSavedPng",
                    "/analyze/showDcmAsPng/{path}",
                    new {controller = "Operations", action = "ShowSavedDcmFileAsPng"},
                    new {path = @"[\w-]+"});
                
                //analyze controller:
                
                routes.MapRoute(
                    "SelectRegion",
                    "/analyze/selectregion/{FileName}",
                    new {controller = "Analyze", action = "SelectRegion"},
                    new {FileName = @"[\w-]+"});
                routes.MapRoute(
                    "StartAnalyzing",
                    "/analyze/startAnalyze",
                    new {controller = "Analyze", action = "StartAnalyzing"});
                routes.MapRoute(
                    "ShowAnalysis",
                    "/analyze/showResult",
                    new {controller = "Analyze", action = "ShowAnalysis"});
                routes.MapRoute(
                    "about",
                    "/about",
                    new {controller = "Home", action = "About"});
                routes.MapRoute(
                    "statistics",
                    "/statistics",
                    new {controller = "Home", action = "Statistics"});
                routes.MapRoute(
                    "contact",
                    "/contact",
                    new {controller = "Home", action = "Contact"});
                routes.MapRoute("analysisStatus", "/analyze/getStatus",
                    new {controller = "Analyze", action = "GetAnalysisStatus"});
                routes.MapRoute(
                    "default",
                    "/",
                    new {controller = "Home", action = "Index"});
            });
        }
    }
}