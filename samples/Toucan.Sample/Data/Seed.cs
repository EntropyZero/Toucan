using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Sample.Models;

namespace Toucan.Sample.Data
{
    public static class Seed
    {
        public static void LoadData(IServiceProvider serviceProvider)
        {
            Task roleTask = AddRoles(serviceProvider);
            roleTask.Wait(); 
            
            Task userTask = AddUsers(serviceProvider);
            userTask.Wait();
        }
        
        private static async Task AddRoles(IServiceProvider serviceProvider)
        {
            string[] roleNames = { "Admin", "Member" };
            
            RoleManager<IdentityRole> roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            foreach(string roleName in roleNames)
            {
                if(! await roleMgr.RoleExistsAsync(roleName))
                {
                    IdentityRole role = new IdentityRole{ Name = roleName};
                    await roleMgr.CreateAsync( role );
                }
            }
        }
        
        private static async Task AddUsers(IServiceProvider serviceProvider)
        {
            string[] userNames = {"member@toucan.sample.com", "admin@toucan.sample.com"};

            UserManager<ApplicationUser> userMgr = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            foreach(string userName in userNames)
            {
                if(userMgr.Users.Where(u => u.UserName == userName).Count() == 0)
                {
                    ApplicationUser user = new ApplicationUser{UserName = userName};                   
                    await userMgr.CreateAsync(user, "P@ssw0rd!");
                    await userMgr.AddToRoleAsync(user, "Member");
                    if(userName.Contains("admin"))
                    {
                        await userMgr.AddToRoleAsync(user, "Admin");
                    }
                }
            }
        }
    }
}