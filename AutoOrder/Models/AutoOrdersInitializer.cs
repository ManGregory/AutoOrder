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
    public class AutoOrdersInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
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

            const string adminUserName = "admin@admin.net";
            if (!context.Users.Any(u => u.Email == adminUserName))
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@admin.net",
                    Email = adminUserName,
                };
                var result = userManager.Create(user, roleName + "123");
                userManager.AddToRole(user.Id, roleName);
            }
        }
    }
}