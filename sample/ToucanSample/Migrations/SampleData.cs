using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using ToucanSample.Models;

namespace ToucanSample.Migrations
{
    internal class SampleData
    {
        internal static void Seed(IServiceProvider provider)
        {
            UserManager<ApplicationUser> userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            
            if(!roleManager.Roles.Any())
            {
                Task task = roleManager.CreateAsync(new IdentityRole("Member"));  
                task.Wait();  
            }
            
            if(!userManager.Users.Any())
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "philmcmillan@hotmail.com", 
                    Email = "philmcmillan@hotmail.com"
                };
                Task task = userManager.CreateAsync(user, "P@ssword1");
                task.Wait();
                
                Task task2 = userManager.AddToRoleAsync(user, "Member");
                task2.Wait();
            }
            
            var context = provider.GetRequiredService<ApplicationDbContext>();
            if(!context.Posts.Any())
            { 
                context.Add<Post>(
                    new Post{
                        Title = "This is a test post",
                        Contents = "Ipso lorum facto....",
                        CreatedAt = DateTime.Now,
                        CreatedBy = userManager.Users.First()
                        });
                context.SaveChanges();
            }
        }
    }
}