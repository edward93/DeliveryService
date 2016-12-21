using System.Security.Claims;
using DAL.Constants;
using DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

namespace DAL.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<Context.DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Context.DbContext context)
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            rm.Create(new IdentityRole(Roles.Member));
            rm.Create(new IdentityRole(Roles.Admin));

            var um = new UserManager<User>(new UserStore<User>(context));
            var user = new User
            {
                UserName = "admin@gmail.com",
                EmailConfirmed = true,
                Email = "admin@gmail.com"
            };

            um.Create(user, "password1");
            var currentUser = um.FindByEmail(user.Email);
            um.AddToRole(currentUser.Id, Roles.Admin);
            um.AddClaim(currentUser.Id, new Claim(ClaimTypes.Role, Roles.Admin));
        }
    }
}
