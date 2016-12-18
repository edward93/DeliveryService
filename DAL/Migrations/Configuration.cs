using System.Security.Claims;
using DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.Context.DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DAL.Context.DbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            rm.Create(new IdentityRole("Member"));
            rm.Create(new IdentityRole("Admin"));

            var um = new UserManager<User>(new UserStore<User>(context));
            var user = new User
            {
                UserName = "admin@gmail.com",
                EmailConfirmed = true,
                Email = "admin@gmail.com"
            };

            um.Create(user, "password1");
            var currentUser = um.FindByEmail(user.Email);
            um.AddToRole(currentUser.Id, "Admin");
            um.AddClaim(currentUser.Id, new Claim(ClaimTypes.Role, "Admin"));
        }
    }
}
