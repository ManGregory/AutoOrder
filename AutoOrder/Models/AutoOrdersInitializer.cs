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

            var r = new Random();

            context.TrailerTypes.Add(new TrailerType
            {
                Id = 1,
                Name = "открытый",
                TransportName = "открытая"
            });
            context.TrailerTypes.Add(new TrailerType
            {
                Id = 2,
                Name = "крытый",
                TransportName = "крытая"
            });

            for (var i = 0; i < 20; i++)
            {
                context.Autoparks.Add(new Autopark
                {
                    Name = string.Format("Автомобиль {0}", i),
                    Trailer = string.Format("Прицеп {0}", i),
                    TrailerCount = 1,
                    TrailerHeight = r.Next(1, 3),
                    TrailerLength = Math.Round(r.Next(5, 10) * r.NextDouble(), 2),
                    TrailerWidth = r.Next(1, 3),
                    TrailerTypeId = r.Next(1, 3)
                });
            }
            context.SaveChanges();
        }
    }
}