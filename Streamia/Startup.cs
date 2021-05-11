using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Streamia.Helpers;
using Streamia.Middlewares;
using Streamia.Models;
using Streamia.Models.Contexts;
using Streamia.Models.Extensions;
using Streamia.Models.Interfaces;
using Streamia.Models.Repositories;
using Streamia.Realtime;
using Streamia.Realtime.Containers;
using Streamia.Realtime.Interfaces;

namespace Streamia
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
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<StreamiaContext>()
            .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddSignalR(options => options.EnableDetailedErrors = true);

            services.AddDbContextPool<StreamiaContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("StreamiaMasterSQL")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton(typeof(IRemoteConnection), typeof(SshContainer));

            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddRazorRuntimeCompilation();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", p => p.RequireClaim("IsAdmin", "true"));
                options.AddPolicy("Reseller", p => p.RequireClaim("IsReseller", "true"));
                options.AddPolicy("Mag", p => p.RequireClaim("AddMag", "true"));
                options.AddPolicy("Enigma", p => p.RequireClaim("AddEnigma", "true"));
            });
        }

        public void Configure
        (
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            UserManager<AppUser> userManager
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            SeedDataExtensions.SeedUsers(userManager);
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<Trial>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<ServerStatusHub>("/server-status-hub");
                endpoints.MapHub<StreamStatusHub>("/stream-status-hub");
                endpoints.MapHub<MovieStatusHub>("/movie-status-hub");
                endpoints.MapHub<ChannelStatusHub>("/channel-status-hub");
                endpoints.MapHub<DirectoryBrowserHub>("/directory-browser-hub");
            });
        }
    }
}
