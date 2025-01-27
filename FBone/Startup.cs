using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FastReport.Data;
using FBone.Database;
using FBone.Database.DB_Helper;
using FBone.Models.NMode;
using FBone.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FBone
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
            //подключаем конфиг из appsetting.json
            Configuration.Bind("FBone", new Config());

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
            services.AddTransient<tUserTable>();
            services.AddTransient<tPositionTable>();
            services.AddTransient<tActCauseTable>();
            services.AddTransient<tActImpactTable>();
            services.AddTransient<tActProtectTable>();
            services.AddTransient<tAreaTable>();
            services.AddTransient<tActTable>();
            services.AddTransient<tActItemsTable>();
            services.AddTransient<TagTable>();
            services.AddTransient<EventTable>();
            services.AddTransient<DeviceTable>();
            services.AddTransient<tFacilityTable>();
            services.AddTransient<ActHistoryTable>();
            services.AddTransient<EmailTemplateTable>();
            services.AddTransient<RequestLogTable>();
            services.AddTransient<AuditTable>();
            services.AddTransient<AuditFileTable>();
            services.AddTransient<DataManager>();            
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddMvcOptions(options=>options.EnableEndpointRouting =false);
            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Config.GetConnectionString()));

            services.Configure<NModeSettings>(Configuration);
            services.AddTransient<NMManager>();
            services.AddFastReport();
            //language settings START
            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();
            var supportedCultures = LanguageHelper.GetAvailableLanguages();
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedLanguages = new List<CultureInfo>();
                    foreach (var item in supportedCultures)
                    {
                        supportedLanguages.Add(new CultureInfo(item));
                    }                    
                    options.SupportedCultures = supportedLanguages;
                    options.SupportedUICultures = supportedLanguages;

                    options.RequestCultureProviders.Clear();
                    options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                    {
                        try
                        {
                            return Task.FromResult(new ProviderCultureResult(UserHelper.GetUserLanguage(context.User.Identity.Name)));
                        }
                        catch (Exception)
                        {
                            return Task.FromResult(new ProviderCultureResult("en"));
                        }
                    }));
                });
            services.AddDataProtection()
                .SetApplicationName("FBone")
                .PersistKeysToFileSystem(new DirectoryInfo("fbone-keys"));
            //language settings END
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseFastReport();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            //language settings START
            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            //language settings END
            app.UseRouting();


            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
