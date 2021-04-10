using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ApplicationShared.Services;
using Core.SharedDomain.Security;
using EntityFrameworkCore;
using EntityFrameworkCore.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Startup
{
    public class Startup
    {
        private const string DefaultCorsPolicyName = "localhost";
        private const string AdminPassword = "P@ssW0rd";
        private const string AllowAllCors = "CorsPolicy";
        private IHostingEnvironment _hostingEnv;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            //if (env.IsDevelopment())
            //{
            //    // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            //    builder.AddUserSecrets<Startup>();
            //}

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            LocalizationConfig.Localize(env);
            _hostingEnv = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //for email 
            services.AddScoped<SmtpClient>((serviceProvider) =>
            {
                //var config = serviceProvider.GetRequiredService<IConfiguration>();
                return new SmtpClient()
                {
                    Host = Configuration.GetValue<String>("Email:Smtp:Host"),
                    Port = Configuration.GetValue<int>("Email:Smtp:Port"),
                    Credentials = new NetworkCredential(
                            Configuration.GetValue<String>("Email:Smtp:Username"),
                            Configuration.GetValue<String>("Email:Smtp:Password")
                        ),
                    EnableSsl = Configuration.GetValue<bool>("Email:Smtp:EnableSsl")
                    //DeliveryMethod = SmtpDeliveryMethod.Network
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            

            services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            //Configure CORS for angular UI

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowOrigin",
            //        builder => builder.WithOrigins("http://localhost:4200"));
            //});
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
    builder =>
    {
       // builder.WithOrigins("https://sy-store.com")
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader().AllowAnyMethod();
    });
            });

            services.AddDbContext<CodeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<CodeContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
                        {
                            // Password settings.
                            options.Password.RequireDigit = false;
                            options.Password.RequireLowercase = false;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequireUppercase = false;
                            options.Password.RequiredLength = 3;
                            options.Password.RequiredUniqueChars = 0;

                            // Lockout settings.
                            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                            options.Lockout.MaxFailedAccessAttempts = 5;
                            options.Lockout.AllowedForNewUsers = true;

                            // User settings.
                            options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                            options.User.RequireUniqueEmail = false;
                        });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.Events = new JwtBearerEvents
                   {
                       OnTokenValidated = context =>
                       {

                           var user = context.Principal.Identity.Name;
                           //Grab the http context user and validate the things you need to
                           //if you are not satisfied with the validation fail the request using the below commented code
                           //context.Fail("Unauthorized");

                           //otherwise succeed the request
                           return Task.CompletedTask;
                       }
                   };
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                          {
                              ValidateIssuer = false,
                              ValidateAudience = false,
                              ValidateLifetime = false,
                              ValidateIssuerSigningKey = true,
                              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                          };
                   });
            //configue services 
            //services.AddTransient<ApplicationShared.Services.IEmailSender, AuthMessageSender>();
            //services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseOpenApi();
            app.UseDeveloperExceptionPage();
            app.UseSwaggerUi3();
            app.UseAuthentication();
            app.UseCors("AllowAllHeaders");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            try
            {
                SeedData.Initialize(app, AdminPassword).Wait();
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }
    }
}
