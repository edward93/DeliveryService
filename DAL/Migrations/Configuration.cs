using System;
using System.Security.Claims;
using DAL.Constants;
using DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using DAL.Enums;

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
            rm.Create(new IdentityRole(Roles.Business));

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

            var adminPerson = new Person
            {
                IsDeleted = false,
                CreatedBy = 2,
                CreatedDt = DateTime.UtcNow,
                DateOfBirth = DateTime.UtcNow.AddYears(-21),
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Admin",
                Sex = Sex.Male,
                Phone = "123456789",
                UpdatedBy = 2,
                UpdatedDt = DateTime.UtcNow,
                UserId = user.Id
            };

            context.Persons.AddOrUpdate(adminPerson);
            context.SaveChanges();
        }
    }
}
