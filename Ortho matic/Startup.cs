using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ortho_matic.Data;
using Ortho_matic.Models;
using System;
using System.Linq;
using System.Text;

namespace Ortho_matic
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );
            });
            services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:ValidAudience"],
                    ValidAudience = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
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

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            UpdateDatabase(app, userManager, roleManager);

            app.UseAuthentication();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void UpdateDatabase(IApplicationBuilder app, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            try
            {
                context.Database.Migrate();

                if (!context.Roles.Any(role => role.Name == "Admin"))
                {
                    var admin = new IdentityRole("Admin");
                    var r = roleManager.CreateAsync(admin).Result;
                }

                if (!context.Roles.Any(role => role.Name == "SubAdmin"))
                {
                    var admin = new IdentityRole("SubAdmin");
                    var r = roleManager.CreateAsync(admin).Result;
                }

                if (!context.Roles.Any(role => role.Name == "Staff"))
                {
                    var staff = new IdentityRole("Staff");
                    var r = roleManager.CreateAsync(staff).Result;
                }

                context.SaveChanges();

                if (!context.Users.Any(obj => obj.Email == "admin@admin.com"))
                {
                    var user = new ApplicationUser
                    {
                        Email = "admin@admin.com",
                        EmployeeName = "admin",
                        UserName = "admin",
                    };

                    var r = userManager.CreateAsync(user, "123456kE@").Result;
                    var e = userManager.AddToRoleAsync(user, "Admin").Result;

                    context.Users.Add(user);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }
    }
}
