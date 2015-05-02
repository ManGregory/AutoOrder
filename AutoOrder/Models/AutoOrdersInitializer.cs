using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mime;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AutoOrder.Models
{
    public class AutoOrdersInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            const string roleName = "admin";
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
            if (!roleManager.RoleExists("client"))
            {
                roleManager.Create(new IdentityRole("client"));
            }

            const string adminUserName = "admin@autoorders.com";
            if (!context.Users.Any(u => u.Email == adminUserName))
            {
                var user = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminUserName,
                };
                userManager.Create(user, "admin123");
                userManager.AddToRole(user.Id, roleName);
            }

            context.TrailerTypes.Add(new TrailerType
            {
                Name = "открытый",
                TransportName = "открытая"
            });
            context.TrailerTypes.Add(new TrailerType
            {
                Name = "крытый",
                TransportName = "крытая"
            });
            context.SaveChanges();
        }
    }
}