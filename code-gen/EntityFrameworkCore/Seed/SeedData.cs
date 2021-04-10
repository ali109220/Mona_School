
using Core.SharedDomain.Security;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seed
{
    public static class SeedData
    {
        public static async Task Initialize(IApplicationBuilder app, string password)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CodeContext>();
                // we are seeding admin user both with the same password.
                // The admin user 

                var adminID = await EnsureUser(serviceScope.ServiceProvider, password, "admin");
                await EnsureRole(serviceScope.ServiceProvider, adminID, "Admin");
            }
        }
        

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string Userpw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();
            IdentityResult IR = null;
            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new User { UserName = UserName };
                IR = await userManager.CreateAsync(user, Userpw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<User>>();

            var user = await userManager.FindByIdAsync(uid);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }
}