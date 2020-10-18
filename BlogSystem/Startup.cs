using System;
using System.Text;
using BlogSystem.Models;
using BlogSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using BlogSystem.Middleware;
using Microsoft.AspNetCore.Http;

namespace BlogSystem
{
    /// <summary>
    /// This class registers the base settings for the application that will apply on startup.
    /// </summary>
    public class Startup
    {
        // Inject dependencies to reach the settings specified in appsettings.json:
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddMvc(option => option.EnableEndpointRouting = false).AddSessionStateTempDataProvider();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ITokenHandler, Services.TokenHandler>();
            services.AddTransient<IAuthHandler, AuthHandler>();

            // Register the database context:
            services.AddDbContext<BlogSystemContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DBConnect")));

            // Register support for JSON Web Tokens:
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                { 
                    options.RequireHttpsMetadata = false; 
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    { 
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    }; 
                });

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseMiddleware<AuthServer>();
            app.UseMvcWithDefaultRoute();
        }

    } // End class.
} // End namespace.
