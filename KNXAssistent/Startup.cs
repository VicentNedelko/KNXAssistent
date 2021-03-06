using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using KNXAssistent.Data;
using KNXManager.FileService;
using KNXManager.BotManager;
using KNXManager.BusConnection;
using MatBlazor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using KNXManager.MessageService;
using KNXManager.HEOSService;
using KNXManager.ACU;
using KNXManager.MonitorService;
using KNXManager.HS;

namespace KNXAssistent
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
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMatBlazor();
            services.AddScoped<IBusCommunicator, BusCommunicator>();;
            services.AddScoped<IBot, Bot>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IMessService, MessService>();
            services.AddScoped<IHeosService, HeosService>();
            services.AddScoped<IAcuErrorHandler, AcuErrorHandler>();
            services.AddScoped<IMonitor, Monitor>();
            services.AddScoped<IHsService, HsService>();

            services
              .AddBlazorise(options =>
              {
                  options.ChangeTextOnKeyPress = true; // optional
              })
              .AddBootstrapProviders()
              .AddFontAwesomeIcons();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
