using CMS.Data;
using CMS.Data.Repositories;
using CMS.Interfaces.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CMS
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
            services.AddControllersWithViews();

            bool useInMemory = Configuration.GetSection("RepositorySettings").GetValue<bool>("UseInMemoryRepository");

            if (!useInMemory && IsDatabaseOnline()) 
            {
                services.AddDbContext<DataContext>(o =>
                    o.UseSqlServer(Configuration.GetSection("RepositorySettings").GetValue<string>("SqlConnection"))
                );
                services.AddScoped<ICourseRepository, CourseRepository>();
            }
            else
            {
                services.AddSingleton<ICourseRepository, CourseRepositoryInMemory>();
            }
            
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public bool IsDatabaseOnline()
        {
            try {
                SqlConnection connection = new SqlConnection(Configuration.GetSection("RepositorySettings").GetValue<string>("SqlConnection"));
                connection.Open();
                return true;
            }
            catch (Exception ignore) 
            { return false; }
        }
    }
}
